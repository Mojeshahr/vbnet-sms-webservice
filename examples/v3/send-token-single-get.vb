' SendTokenSingle با GET - همان ارسال قالب، با ورودی در نشانی.
'
' برای آزمایش دستی مناسب است، برای محیط عملیاتی نه: در GET هم کلید حساب و هم
' مقدار رمز یک‌بارمصرف داخل نشانی می‌نشینند و در لاگ وب‌سرور و هدر Referer
' ثبت می‌شوند. واریانت POST را بردارید.
'
' جز کتابخانه استاندارد دات‌نت به چیزی وابسته نیست. کپی کنید و در پروژه
' خودتان اجرا کنید.
'
'   PAYAM_RESAN_API_KEY=... dotnet run -p:Example=examples/v3/send-token-single-get.vb

' docs:start
Option Strict On
Option Infer On

Imports System.Collections.Generic
Imports System.Net.Http
Imports System.Text.Json.Nodes
Imports System.Threading.Tasks

Module Program

    Function Main() As Integer
        Return SendTokenSingleGet().GetAwaiter().GetResult()
    End Function

    Async Function SendTokenSingleGet() As Task(Of Integer)
        Dim query As New Dictionary(Of String, String) From {
            {"ApiKey", Environment.GetEnvironmentVariable("PAYAM_RESAN_API_KEY")},
            {"TemplateKey", "verifycode"},
            {"Destination", "9121112222"},
            {"p1", "123456"}
        }

        Dim url = "https://api.sms-webservice.com/api/V3/SendTokenSingle?" &
            Await New FormUrlEncodedContent(query).ReadAsStringAsync()

        Using http As New HttpClient With {.Timeout = TimeSpan.FromSeconds(30)}
            Dim response = JsonNode.Parse(Await http.GetStringAsync(url))

            Dim success = response?("Success")
            If success Is Nothing OrElse Not success.GetValue(Of Boolean)() Then
                Console.Error.WriteLine($"ناموفق. کد {response?("ErrorCode")}: {response?("Error")}")
                Return 1
            End If

            For Each message In response("Result").AsArray()
                Console.WriteLine($"شناسه {message("Id")}، متن نهایی: {message("FinalText")}")
            Next
        End Using

        Return 0
    End Function

End Module
' docs:end
