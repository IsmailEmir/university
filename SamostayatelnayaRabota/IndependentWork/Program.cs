using System.Net;
using System.Text;

namespace IndependentWork;

class Program
{
    static readonly List<Goal> Goals = new();
    static int nextId = 1;
    static readonly object Lock = new();
    static readonly string[] Periods = { "week", "month", "year" };
    static readonly string[] Statuses = { "active", "completed" };

    static void Main()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Goals tracker: http://localhost:8080/");
        Console.WriteLine("Press Ctrl+C to stop.");

        while (true)
        {
            var context = listener.GetContext();
            ThreadPool.QueueUserWorkItem(_ => Handle(context));
        }
    }

    static void Handle(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;
            string path = request.Url?.AbsolutePath ?? "/";
            string method = request.HttpMethod.ToUpperInvariant();

            if (path == "/") { Redirect(response, "/goals"); return; }

            if (path == "/goals" && method == "GET") { GoalsList(response, request); return; }
            if (path == "/goals/details" && method == "GET") { GoalDetails(response, request); return; }
            if (path == "/goals/create" && method == "POST") { GoalCreate(response, request); return; }
            if (path == "/goals/update" && method == "PUT") { GoalUpdate(response, request); return; }

            NotFound(response, "Route not found");
        }
        catch (Exception ex)
        {
            try { BadRequest(context.Response, "Server error: " + ex.Message); }
            catch { }
        }
    }

    static string Page(string body)
    {
        return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Goals</title></head><body>" + body + "</body></html>";
    }

    static void GoalsList(HttpListenerResponse response, HttpListenerRequest request)
    {
        string? periodFilter = request.QueryString["period"];
        if (!string.IsNullOrEmpty(periodFilter) && !IsValidPeriod(periodFilter))
        {
            BadRequest(response, "Invalid period filter. Allowed: week, month, year.");
            return;
        }

        List<Goal> goals;
        lock (Lock) goals = new List<Goal>(Goals);
        if (!string.IsNullOrEmpty(periodFilter))
            goals = goals.FindAll(g => g.Period == periodFilter);

        var sb = new StringBuilder();
        sb.Append("<h1>Personal Goals</h1>");

        sb.Append("<form method=\"get\" action=\"/goals\">");
        sb.Append("<label>Period: <select name=\"period\">");
        sb.Append("<option value=\"\">all</option>");
        foreach (var p in Periods)
        {
            string sel = periodFilter == p ? " selected" : "";
            sb.Append($"<option value=\"{Enc(p)}\"{sel}>{Enc(p)}</option>");
        }
        sb.Append("</select></label> ");
        sb.Append("<button type=\"submit\">Filter</button></form>");

        sb.Append("<h2>Counts by period</h2>");
        foreach (var p in Periods)
        {
            int completed = 0, uncompleted = 0;
            lock (Lock)
            {
                foreach (var g in Goals)
                {
                    if (g.Period != p) continue;
                    if (g.Status == "completed") completed++;
                    else uncompleted++;
                }
            }
            sb.Append($"<p>{Enc(p)}: completed {completed}, uncompleted {uncompleted}</p>");
        }

        sb.Append("<h2>Goals</h2>");
        sb.Append("<ul>");
        foreach (var g in goals)
            sb.Append($"<li>{g.Id}. {Enc(g.Title)} ({Enc(g.Period)}, {Enc(g.Status)}) - <a href=\"/goals/details?id={g.Id}\">details</a></li>");
        sb.Append("</ul>");

        sb.Append("<h2>Create goal</h2>");
        sb.Append("<form method=\"post\" action=\"/goals/create\">");
        sb.Append("<label>Title: <input name=\"title\" minlength=\"5\" maxlength=\"100\" required></label><br>");
        sb.Append("<label>Period: <select name=\"period\">");
        foreach (var p in Periods)
            sb.Append($"<option value=\"{Enc(p)}\">{Enc(p)}</option>");
        sb.Append("</select></label><br>");
        sb.Append("<button type=\"submit\">Create</button></form>");

        WriteHtml(response, 200, Page(sb.ToString()));
    }

    static void GoalDetails(HttpListenerResponse response, HttpListenerRequest request)
    {
        if (!TryParseId(request.QueryString["id"], out int id))
        {
            BadRequest(response, "Missing or invalid id.");
            return;
        }
        Goal? goal;
        lock (Lock) goal = Goals.Find(g => g.Id == id);
        if (goal == null)
        {
            NotFound(response, "Goal not found.");
            return;
        }

        var sb = new StringBuilder();
        sb.Append($"<h1>Goal #{goal.Id}</h1>");
        sb.Append($"<p>Title: {Enc(goal.Title)}</p>");
        sb.Append($"<p>Period: {Enc(goal.Period)}</p>");
        sb.Append($"<p>Status: {Enc(goal.Status)}</p>");

        sb.Append("<h2>Update</h2>");
        sb.Append("<form id=\"upd\">");
        sb.Append($"<label>Title: <input id=\"title\" value=\"{Enc(goal.Title)}\" minlength=\"5\" maxlength=\"100\" required></label><br>");
        sb.Append("<label>Period: <select id=\"period\">");
        foreach (var p in Periods)
        {
            string sel = goal.Period == p ? " selected" : "";
            sb.Append($"<option value=\"{Enc(p)}\"{sel}>{Enc(p)}</option>");
        }
        sb.Append("</select></label><br>");
        sb.Append("<label>Status: <select id=\"status\">");
        foreach (var s in Statuses)
        {
            string sel = goal.Status == s ? " selected" : "";
            sb.Append($"<option value=\"{Enc(s)}\"{sel}>{Enc(s)}</option>");
        }
        sb.Append("</select></label><br>");
        sb.Append("<button type=\"submit\">Update</button></form>");
        sb.Append("<p id=\"msg\"></p>");
        sb.Append("<script>");
        sb.Append("document.getElementById('upd').addEventListener('submit',async(e)=>{");
        sb.Append("e.preventDefault();");
        sb.Append("const b=new URLSearchParams({title:document.getElementById('title').value,period:document.getElementById('period').value,status:document.getElementById('status').value});");
        sb.Append($"const r=await fetch('/goals/update?id={goal.Id}',{{method:'PUT',headers:{{'Content-Type':'application/x-www-form-urlencoded'}},body:b.toString()}});");
        sb.Append("document.getElementById('msg').textContent=r.ok?'Updated (204)':'Error: '+r.status;");
        sb.Append("if(r.ok)setTimeout(()=>location.reload(),500);");
        sb.Append("});</script>");

        sb.Append("<p><a href=\"/goals\">Back to list</a></p>");

        WriteHtml(response, 200, Page(sb.ToString()));
    }

    static void GoalCreate(HttpListenerResponse response, HttpListenerRequest request)
    {
        var form = ReadForm(request);
        string title = (form.TryGetValue("title", out var t) ? t : "").Trim();
        string period = form.TryGetValue("period", out var p) ? p : "";

        if (!ValidTitle(title)) { BadRequest(response, "Title must be 5-100 characters."); return; }
        if (!IsValidPeriod(period)) { BadRequest(response, "Period must be week, month or year."); return; }

        lock (Lock)
            Goals.Add(new Goal { Id = nextId++, Title = title, Period = period, Status = "active" });

        Redirect303(response, "/goals");
    }

    static void GoalUpdate(HttpListenerResponse response, HttpListenerRequest request)
    {
        if (!TryParseId(request.QueryString["id"], out int id))
        {
            BadRequest(response, "Missing or invalid id.");
            return;
        }
        var form = ReadForm(request);
        string title = (form.TryGetValue("title", out var t) ? t : "").Trim();
        string period = form.TryGetValue("period", out var p) ? p : "";
        string status = form.TryGetValue("status", out var s) ? s : "";

        if (!ValidTitle(title)) { BadRequest(response, "Title must be 5-100 characters."); return; }
        if (!IsValidPeriod(period)) { BadRequest(response, "Period must be week, month or year."); return; }
        if (!IsValidStatus(status)) { BadRequest(response, "Status must be active or completed."); return; }

        lock (Lock)
        {
            var goal = Goals.Find(g => g.Id == id);
            if (goal == null)
            {
                NotFound(response, "Goal not found.");
                return;
            }
            goal.Title = title;
            goal.Period = period;
            goal.Status = status;
        }

        response.StatusCode = 204;
        response.Close();
    }

    static bool ValidTitle(string t) => t.Length >= 5 && t.Length <= 100;
    static bool IsValidPeriod(string p) => Array.IndexOf(Periods, p) >= 0;
    static bool IsValidStatus(string s) => Array.IndexOf(Statuses, s) >= 0;

    static bool TryParseId(string? s, out int id)
    {
        id = 0;
        return int.TryParse(s, out id) && id > 0;
    }

    static Dictionary<string, string> ReadForm(HttpListenerRequest request)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        using var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8);
        string body = reader.ReadToEnd();
        if (string.IsNullOrEmpty(body)) return result;
        foreach (var pair in body.Split('&'))
        {
            if (string.IsNullOrEmpty(pair)) continue;
            var kv = pair.Split('=', 2);
            string key = Decode(kv.Length > 0 ? kv[0] : "");
            string val = kv.Length > 1 ? Decode(kv[1]) : "";
            result[key] = val;
        }
        return result;
    }

    static string Decode(string s) => Uri.UnescapeDataString(s.Replace('+', ' '));
    static string Enc(string s) => WebUtility.HtmlEncode(s);

    static void WriteHtml(HttpListenerResponse response, int status, string html)
    {
        response.StatusCode = status;
        response.ContentType = "text/html; charset=utf-8";
        byte[] buf = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buf.Length;
        response.OutputStream.Write(buf, 0, buf.Length);
        response.Close();
    }

    static void Redirect(HttpListenerResponse response, string location)
    {
        response.StatusCode = 302;
        response.RedirectLocation = location;
        response.Close();
    }

    static void Redirect303(HttpListenerResponse response, string location)
    {
        response.StatusCode = 303;
        response.RedirectLocation = location;
        response.Close();
    }

    static void BadRequest(HttpListenerResponse response, string msg)
    {
        WriteHtml(response, 400, Page("<h1>400 Bad Request</h1><p>" + Enc(msg) + "</p>"));
    }

    static void NotFound(HttpListenerResponse response, string msg)
    {
        WriteHtml(response, 404, Page("<h1>404 Not Found</h1><p>" + Enc(msg) + "</p>"));
    }
}
