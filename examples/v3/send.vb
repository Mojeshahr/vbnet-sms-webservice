' Send - ساده‌ترین ارسال، یک متن به چند شماره با یک درخواست GET.
'
' برای آزمایش سریع خوب است. در محیط عملیاتی SendBulk را بردارید: کلید را از
' نشانی بیرون می‌برد و برای هر گیرنده شناسه پی‌گیری می‌پذیرد.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... PAYAM_RESAN_SENDER=... dotnet run -p:Example=examples/v3/send.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return Send().GetAwaiter().GetResult()
    End Function

    Async Function Send() As Task(Of Integer)
        Dim query As New Dictionary(Of String, String) From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"Sender", Environment.GetEnvironmentVariable("PAYAM_RESAN_SENDER")},
            {"Text", "کد تأیید شما ۱۲۳۴۵۶ است"},
            {"Recipients", "9121112222,9121113333"}
        }

        ' FormUrlEncodedContent دقیقاً یک بار encode می‌کند. اگر متن را خودتان هم
        ' پیش از این UrlEncode کنید، پیامک با نویسه‌های %D8 به گوشی می‌رسد.
        Dim url = "https://api.sms-webservice.com/api/V3/Send?" &
            Await New FormUrlEncodedContent(query).ReadAsStringAsync()

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim response = JsonNode.Parse(Await http.GetStringAsync(url))

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each message In response("Result").AsArray()
                Console.WriteLine($"شناسه {message("Id")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
