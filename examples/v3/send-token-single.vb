' SendTokenSingle - ارسال قالب به یک شماره، با بدنه JSON.
'
' مسیر معمول رمز یک‌بارمصرف. خط فرستنده ورودی ندارد؛ سامانه آن را از روی خود
' قالب برمی‌دارد. همین واریانت POST را به کار ببرید، نه GET: در GET هم کلید
' حساب و هم خود رمز داخل نشانی و لاگ وب‌سرور می‌نشینند.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/send-token-single.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return SendTokenSingle().GetAwaiter().GetResult()
    End Function

    Async Function SendTokenSingle() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"TemplateKey", "verifycode"},
            {"Destination", 9121112222L},
            {"p1", "123456"}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/SendTokenSingle", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            ' این متد UserTraceId در ورودی ندارد، پس در پاسخ null برمی‌گردد. اگر
            ' شناسه پی‌گیری لازم دارید، SendTokenMulti را حتی برای یک گیرنده هم
            ' می‌شود به کار برد.
            For Each message In response("Result").AsArray()
                Console.WriteLine($"شناسه {message("Id")} از خط {message("Sender")}")
                Console.WriteLine($"متن نهایی: {message("FinalText")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
