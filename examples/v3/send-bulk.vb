' SendBulk - یک متن به چند گیرنده، هر کدام با شناسه پی‌گیری خودتان.
'
' روش پیشنهادی برای ارسال عملیاتی. کلید در بدنه درخواست می‌رود نه در نشانی،
' و برای هر گیرنده UserTraceId می‌پذیرد تا گزارش تحویل را بدون نگه‌داشتن Id
' سامانه بگیرید.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... PAYAM_RESAN_SENDER=... dotnet run -p:Example=examples/v3/send-bulk.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Net.Http
Imports System.Text
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return SendBulk().GetAwaiter().GetResult()
    End Function

    Async Function SendBulk() As Task(Of Integer)
        Dim payload As New JsonObject From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"Sender", Long.Parse(Environment.GetEnvironmentVariable("PAYAM_RESAN_SENDER"))},
            {"Text", "سفارش شما ثبت شد."},
            {"Recipients", New JsonArray(
                New JsonObject From {{"Destination", 9121112222L}, {"UserTraceId", 1001L}},
                New JsonObject From {{"Destination", 9121113333L}, {"UserTraceId", 1002L}})}
        }

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim body As New StringContent(payload.ToJsonString(), Encoding.UTF8, "application/json")
            Dim answer = Await http.PostAsync("https://api.sms-webservice.com/api/V3/SendBulk", body)
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
