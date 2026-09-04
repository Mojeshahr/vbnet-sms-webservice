<div align="center">

<a href="https://payam-resan.com">
  <img src=".github/assets/logo.svg" width="64" height="64" alt="پیام رسان">
</a>

<h1>نمونه‌کدهای VB.NET وب‌سرویس پیام رسان</h1>

اتصال به وب‌سرویس <a href="https://payam-resan.com"><b>پنل پیامکی پیام رسان</b></a> با VB.NET و دات‌نت<br>
یک فایل قابل اجرا به‌ازای هر متد سرویس، بدون هیچ وابستگی

[![API](https://img.shields.io/badge/API-V3-0a7cbd)](https://payam-resan.com)
[![.NET](https://img.shields.io/badge/.NET-10-512bd4)](https://dotnet.microsoft.com)
[![Dependencies](https://img.shields.io/badge/dependencies-none-2ea44f)](#شروع-سریع)
[![License](https://img.shields.io/badge/license-MIT-6e7781)](LICENSE)

<b>فارسی</b> · <a href="README.en.md">English</a>

</div>

<sub>دنبال زبان دیگری هستید؟ همین نمونه‌ها برای زبان‌های دیگر هم در
[github.com/Mojeshahr](https://github.com/Mojeshahr) هست.</sub>

---

## شروع سریع

```bash
git clone https://github.com/Mojeshahr/vbnet-sms-webservice.git
cd vbnet-sms-webservice

export PAYAM_RESAN_API_KEY='123456-XXXXXXXXXXXXXXX'
export PAYAM_RESAN_SENDER='30004040'

dotnet run -p:Example=examples/v3/account-info.vb
```

هیچ بسته‌ای از NuGet لازم نیست. هر نمونه فقط از `HttpClient` و
`System.Text.Json` استفاده می‌کند که هر دو در خود دات‌نت هستند.

با `account-info.vb` شروع کنید: چیزی ارسال نمی‌کند، اعتباری مصرف نمی‌کند، و اگر
جواب داد یعنی کلید و اتصال هر دو سالم‌اند.

## چرا یک فایل پروژه اینجا هست

زبان VB برنامه تک‌فایلی ندارد. در C# می‌شود یک فایل `.cs` را مستقیم اجرا کرد،
ولی کامپایلر VB برای هر خط کد یک فایل پروژه می‌خواهد. پس `run.vbproj` در ریشه
نشسته و تنها کارش این است که هر بار دقیقاً همان نمونه‌ای را که نام می‌برید
کامپایل کند و اجرا کند.

خود نمونه‌ها به این فایل وابسته نیستند. هرکدام یک `Module` کامل است و اگر داخل
هر پروژه Console یا WinForms دیگری بگذاریدش، بدون تغییر کار می‌کند.

## پیش از ارسال واقعی

یک سرور آزمایشی هست که مثل سرور عملیاتی جواب می‌دهد ولی پیامکی نمی‌فرستد و
اعتباری مصرف نمی‌کند. کافی است `V3` در نشانی را با `V3SandBox` عوض کنید. تنها
استثنا `TokenList` است که روی آن سرور پیاده نشده.

## متدها

<div dir="rtl">

| نمونه | متد | کار |
|---|---|---|
| [account-info.vb](examples/v3/account-info.vb) | `AccountInfo` | اعتبار و خطوط فعال |
| [send.vb](examples/v3/send.vb) | `Send` | ارسال ساده با `GET` |
| [send-bulk.vb](examples/v3/send-bulk.vb) | `SendBulk` | یک متن به چند گیرنده، با شناسه پی‌گیری |
| [send-multiple.vb](examples/v3/send-multiple.vb) | `SendMultiple` | متن جدا برای هر گیرنده |
| [token-list.vb](examples/v3/token-list.vb) | `TokenList` | فهرست قالب‌ها |
| [send-token-single.vb](examples/v3/send-token-single.vb) | `SendTokenSingle` | ارسال قالب به یک شماره |
| [send-token-single-get.vb](examples/v3/send-token-single-get.vb) | `SendTokenSingle` | همان، با `GET` |
| [send-token-multi.vb](examples/v3/send-token-multi.vb) | `SendTokenMulti` | یک قالب، چند گیرنده |
| [status-by-id.vb](examples/v3/status-by-id.vb) | `StatusById` | وضعیت با شناسه سامانه |
| [status-by-user-trace-id.vb](examples/v3/status-by-user-trace-id.vb) | `StatusByUserTraceId` | وضعیت با شناسه خودتان |
| [get-inbox.vb](examples/v3/get-inbox.vb) | `GetInbox` | پیامک‌های رسیده |

</div>

## اگر روی دات‌نت فریم‌ورک هستید

نمونه‌ها روی دات‌نت ۱۰ نوشته و آزموده شده‌اند. کدشان روی دات‌نت فریم‌ورک ۴.۸ هم
بدون تغییر کامپایل می‌شود، با یک تفاوت: آنجا `System.Text.Json` جزو خود
فریم‌ورک نیست و باید بسته‌اش را از NuGet بگیرید.

## استفاده در پروژه خودتان

نمونه‌ها عمداً به هیچ چیز این مخزن وابسته نیستند، پس کپی‌کردن بدنه فایل داخل
سرویس خودتان کافی است. در یک فرم WinForms، بدنه تابع async را مستقیم داخل یک
`Async Sub Button_Click` بگذارید و `Await`ها را دست نزنید.

اگر ترجیح می‌دهید با کلاس‌های نوع‌دار کار کنید به‌جای `JsonNode`، فقط بخش
خواندن پاسخ را عوض کنید؛ شکل درخواست همان است:

```vb
Public Class SendResult
    Public Property Id As Long
    Public Property UserTraceId As Long?
End Class

Public Class SendResponse
    Public Property Success As Boolean
    Public Property ErrorCode As Integer?
    Public Property [Error] As String
    Public Property Result As SendResult()
End Class
```

قلاب دور `[Error]` لازم است، چون `Error` در VB کلیدواژه است.

یک بسته نصب‌شدنی NuGet هم در برنامه هست و در مخزن جداگانه‌ای منتشر می‌شود.

## چند نکته که وقت‌تان را می‌خرد

**متد `EnsureSuccessStatusCode` اینجا چیزی ثابت نمی‌کند.** سرویس همیشه `200`
برمی‌گرداند، حتی وقتی کلید اشتباه است. تصمیم را از فیلد `Success` بگیرید.

**بررسی `Success` را دو مرحله بنویسید، نه یک مقایسه.** در VB مقایسه با یک مقدار
تهی، خودش تهی می‌شود و `If` آن را به شاخه `Else` می‌فرستد. یعنی
`If response?("Success")?.GetValue(Of Boolean)() <> True` پاسخی را که اصلاً
فیلد `Success` ندارد بی‌صدا رد می‌کند. اول وجود فیلد را بسنجید، بعد مقدارش را،
همان‌طور که هر نمونه اینجا می‌کند.

**نقطه ورود VB نمی‌تواند `Async` باشد.** به همین دلیل در هر نمونه یک `Main`
همگام هست که یک تابع async را صدا می‌زند، و `GetAwaiter().GetResult()` فقط
همان یک جا دیده می‌شود. در برنامه کنسول بی‌خطر است؛ همان فراخوانی داخل WinForms
یا ASP.NET برنامه را قفل می‌کند، پس به بدنه منتقلش نکنید.

**دو خط `Option` بالای فایل را نگه دارید.** خیلی از پروژه‌های VB، به‌ویژه
آن‌هایی که از دات‌نت فریم‌ورک آمده‌اند، `Option Infer` را خاموش دارند و بدون آن
دو خط، هر `Dim x = ...` در این فایل‌ها خطای کامپایل می‌دهد.

**شماره گیرنده صفر ابتدایی ندارد.** یعنی `9121112222` یا با کد کشور
`989121112222`. شماره‌ای که با `9` یا `989` شروع نشود کد خطای `13` می‌گیرد.

**متن را دوباره encode نکنید.** در `send.vb` کلاس `FormUrlEncodedContent` خودش
یک بار این کار را می‌کند. اگر پیش از آن هم `UrlEncode` کنید، پیامک با نویسه‌های
`%D8` به گوشی می‌رسد.

**برای هر گیرنده یک `UserTraceId` یکتا بفرستید.** بعد از یک timeout، این تنها
راه فهمیدن این است که پیامک ثبت شده یا نه.

## امنیت کلید

کلید یک راز است. در مخزن کد، در جاوااسکریپت مرورگر و در بسته اپلیکیشن موبایل
نباید قرار بگیرد. جای آن متغیر محیطی است، همان‌طور که همه نمونه‌ها می‌خوانندش.

اگر کلیدی لو رفت، از پنل یکی تازه بسازید. کلید حذف‌شده برنمی‌گردد.

## ساختار

<div dir="rtl">

| مسیر | چه چیزی دارد |
|---|---|
| `examples/v3/` | یک نمونه مستقل به‌ازای هر عملیات سرویس |
| `run.vbproj` | اجراکننده، و تنها فایلی که خودش نمونه نیست |
| `.env.example` | نمونه متغیرهای محیطی |

</div>

عدد `v3` در مسیر عمدی است. نسخه تازه سرویس یعنی پوشه `examples/v<n>/` تازه، و
پوشه موجود دست‌نخورده می‌ماند.

## مستندات و پشتیبانی

راهنمای کامل وب‌سرویس در [docs.payam-resan.com](https://docs.payam-resan.com)
است. توصیف ماشین‌خوان OpenAPI هم در
[sms-webservice-spec](https://github.com/Mojeshahr/sms-webservice-spec).

## مجوز

منتشرشده با مجوز MIT. متن کامل در [`LICENSE`](LICENSE).
