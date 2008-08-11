@ECHO OFF
cls
echo Would you like to take a poll or create a new one?
echo 1) Start Client
echo 2) Start Editor
CHOICE /N /C:12 /M "Enter choice (1 or 2):" %1
cls
IF ERRORLEVEL ==2 GOTO TWO
IF ERRORLEVEL ==1 GOTO ONE
:TWO
java -cp poll.jar;lib\xpilotpanel-lib.jar;lib\log4j-1.2.15.jar ilsrep.poll.client.PollEditor
GOTO END
:ONE
java -cp poll.jar;lib\xpilotpanel-lib.jar;lib\log4j-1.2.15.jar ilsrep.poll.client.PollClient
:END