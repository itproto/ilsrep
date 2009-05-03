class HtmlUtils():
    def generateStart(self, title):
        output = """
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html>

<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>""" + title + """</title>
<meta name="keywords" content="tabsender, ils, jteam" />
<meta name="description" content="tabsender, jteam, firefox, firefox3, plugin, interlogic, ils" />
<link rel="stylesheet" type="text/css" href="html/class.css" />
</head>

<body>

<div id="outer">

    <div id="header">
        <h1><span>Tab</span><strong>Sender</strong> server</h1>

        <div id="menu">
        """
        return output

    def generateMenuEnd(self):
        output = """
        </div>
    </div>

    <div id="inner">

        <div id="main">
            <div id="xbgA"></div>
    
            <div id="main_inner">

                <!-- Main start -->
        """
        return output

    def generateMainPartEnd(self):
        output = """
                <!-- Main End -->
                <div class="foot"></div>                
            </div>

        </div>

        <div id="side">
            <!-- Side start -->
        """
        return output

    def generateEnd(self):
        output = """
            <!-- Side end -->
        </div>

        <div  class="foot"></div>
    </div>

</div>
<div id="outer2"></div>

<div id="footer">
    &copy; 2009 InterLogic. TabSender server.
</div>
</body>
</html>
        """
        return output
    
    def generateDefaultMenu(self):
        output = """
                <ul>
                <li><a href="/home.html">Project home</a></li>
                <li><a href="/tabinfo.html">My account</a></li>
                </ul>
        """
        return output
