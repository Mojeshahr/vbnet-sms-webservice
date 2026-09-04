' SendTokenMulti - یک قالب، چند گیرنده، مقادیر متفاوت.
'
' پارامترها اینجا آرایه‌اند، نه p1 تا p10. درایه اول به {1} می‌نشیند، دومی به
' {2} و همین‌طور تا آخر: ترتیب از شماره جای‌گاه می‌آید، نه از جایی که در متن
' قالب دیده می‌شود.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/send-token-multi.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return SendTokenMulti().GetAwaiter().GetResult()
    End Function

    Async Function SendTokenMulti() As Task(Of Integer)
        ' قالب نمونه: «مرسوله شما از {2} تحویل پست شد. بارکد مرسوله پستی: {1}»
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"TemplateKey", "postcode"},
            {"Recipients", New JsonArray(
                New JsonObject From {
                    {"Destination", 9121112222L},
                    {"UserTraceId", 1001L},
                    {"Parameters", New JsonArray(JsonValue.Create("BARCODE-AAA"), JsonValue.Create("شیراز"))}},
                New JsonObject From {
                    {"Destination", 9121113333L},
                    {"UserTraceId", 1002L},
                    {"Parameters", New JsonArray(JsonValue.Create("BARCODE-BBB"), JsonValue.Create("تبریز"))}})}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/SendTokenMulti", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each message In response("Result").AsArray()
                Console.WriteLine($"{message("UserTraceId")} => {message("FinalText")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
