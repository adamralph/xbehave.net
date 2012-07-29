echo off
..\packages\xunit.runners.1.9.1\tools\xunit.console.clr4.exe "test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.dll" /noshadow /nunit "test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.Results.xml" /html "test\Xbehave.Test.Acceptance.Net40\bin\Debug\Xbehave.Test.Acceptance.Results.html"
pause