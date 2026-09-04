' SendMultiple - متن و خط فرستنده جدا برای هر گیرنده.
'
' برای پیام‌های شخصی‌سازی‌شده که با یک قالب ثابت پوشش داده نمی‌شوند. برخلاف
' SendBulk، اینجا Text و Sender در سطح هر گیرنده تعریف می‌شوند.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... PAYAM_RESAN_SENDER=... dotnet run -p:Example=examples/v3/send-multiple.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return SendMultiple().GetAwaiter().GetResult()
    End Function

    Async Function SendMultiple() As Task(Of Integer)
        Dim sender = Long.Parse(Environment.GetEnvironmentVariable("PAYAM_RESAN_SENDER"))

        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"Recipients", New JsonArray(
                New JsonObject From {
                    {"Sender", sender},
                    {"Destination", 9121112222L},
                    {"Text", "آقای محمدی، سفارش شما ارسال شد."},
                    {"UserTraceId", 1001L}},
                New JsonObject From {
                    {"Sender", sender},
                    {"Destination", 9121113333L},
                    {"Text", "خانم رضایی، سفارش شما ارسال شد."},
                    {"UserTraceId", 1002L}})}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/SendMultiple", body)
            Dim response = JsonNode.Parse(Await answer.Content.ReadAsStringAsync())

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each message In response("Result").AsArray()
                Console.WriteLine($"{message("UserTraceId")} => شناسه {message("Id")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
