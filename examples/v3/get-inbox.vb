' GetInbox - پیامک‌هایی که کاربران به خطوط حساب شما فرستاده‌اند.
'
' این یک استعلام است، نه webhook: سامانه چیزی به سرور شما نمی‌فرستد و باید
' خودتان دوره‌ای صدایش بزنید. فاصله را کمتر از چند دقیقه نگذارید، وگرنه به
' خطای ۲۰ می‌خورید.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/get-inbox.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return GetInbox().GetAwaiter().GetResult()
    End Function

    Async Function GetInbox() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/GetInbox", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each sms In response("Result").AsArray()
                ' نام فیلد فرستنده در خود سرویس Form است، نه From. دنبال From نگردید.
                Console.WriteLine($"{sms("Time")}  {sms("Form")} -> {sms("To")}: {sms("Text")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
