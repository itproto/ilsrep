from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

from Categories import Categories
from Categories import deleteSession
from Categories import deleteURL

from urllib import quote_plus

class TabInfoPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    user = users.get_current_user()

    if user:
        output = """<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>

<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>TabSender</title>
<meta name="keywords" content="tabsender, ils, jteam" />
<meta name="description" content="jteam tabsender" />
<link rel="stylesheet" type="text/css" href="html/class.css" />
</head>

<body>

<div id="outer">

    <div id="header">
        <h1><span>Tab</span><strong>Sender</strong> server</h1>

        <div id="menu">
            <ul>
                <li><a href="/html/info.html">Project home</a></li>
                <li><a href="/tabinfo.html">My account</a></li>
            </ul>
        </div>
    </div>

    <div id="inner">

        <div id="main">
            <div id="xbgA"></div>
    
            <div id="main_inner">

                <!-- Main start -->
"""

        type = self.request.get('type')

        if (not type) or (type == 'user'):
            output += '<h1><span class="colouredText">' + user.nickname() + '</span>\'s stored sessions on server:</h1><ul>'

            categories_all = db.GqlQuery("SELECT * FROM Categories where user='" + user.nickname() + "'")

            ids = []

            for cat in categories_all:
                if cat.id not in ids:
                    output += '<li><a href="/tabinfo.html?type=session&id=' + str(cat.id) +'">' + cat.name + '</a>\n'
                    output += '<a href="/tabinfo.html?type=removesession&id='+ str(cat.id) + '"><img class="imageURL" src="/html/images/cancel.png" /></a>' + '</li>\n'
#                    output += '[' + '<a href="/tabinfo.html?type=removesession&id='+ str(cat.id) + '">Delete</a>' + ']</li>\n'
                    ids.append(cat.id)

            output += '</ul>'
        elif type == 'session':
            id = self.request.get('id')
            if not id:
                output += 'Id not specified!'
            else:
                links = []

                linksForThisSession = db.GqlQuery("select * FROM Categories where id=" + id)

                name = ""

                for link in linksForThisSession:
                    if name == "":
                        name = link.name
                    links.append(link.url)

                if len(links) > 0:
                    output += '<h1>URLs for session <span class="colouredText">' + name + ':</span></h1><ul>'
                    for link in links:
                        output += '<li><a href="' + link + '">' + link + '</a>\n'
                        output += '<a href="/tabinfo.html?type=removeurl&id='+ str(id) + '&url=' + quote_plus(link) + '"><img class="imageURL" src="/html/images/cancel.png" /></a>' + '</li>\n'
#                        output += '[' + '<a href="/tabinfo.html?type=removeurl&id='+ str(id) + '&url=' + quote_plus(link) + '">Delete</a>' + ']</li>\n'
                    output += '</ul>'
                else:
                    output += '<h1>Session was deleted!</h1> '

                output += '<form method="get" action="/tabinfo.html">' + '\n'
                output += '<input type="hidden" name="type" value="posturl" />' + '\n'
                output += '<input type="hidden" name="id" value="' + id +'" />' + '\n'
                output += '<input type="text" size="30" name="url" />' + '\n'
                output += '<input type="submit" value="Add URL" /> </form>' + '\n'

                output += '<a href="/tabinfo.html"><img class="imageURL" src="/html/images/arrow_left.png" /> Back</a>'
        elif type == "removesession":
            id = self.request.get('id')
            if id:
                deleteSession(id)
#            output += '<h1>Session removed!<h1>'
#            output += '<a href="/tabinfo.html">Back</a>'
            self.redirect("/tabinfo.html")
        elif type == "removeurl":
            id = self.request.get('id')
            url = self.request.get('url')

            if id and url:
                deleteURL(id, url)
#                output += '<h1>URL removed!</h1>'

#            output += '<a href="/tabinfo.html?type=session&id=' + id + '">Back</a>'
            self.redirect("/tabinfo.html?type=session&id=" + id)
        elif type == "posturl":
            id = self.request.get('id')
            url = self.request.get('url')

            if id and url:
                cat = Categories()
                cat.user = user.nickname()
                cat.url = url

                name_select = db.GqlQuery("SELECT * FROM Categories WHERE id=" + id +" LIMIT 1")
                cat.name = name_select[0].name
                cat.id = name_select[0].id

                cat.put()

                self.redirect("/tabinfo.html?type=session&id=" + id)

        output += """
                <!-- Main End -->
                <div class="foot"></div>                
            </div>

        </div>

        <div id="side">
            <!-- Side start -->
            <ul>
        """

        categories_all = db.GqlQuery("SELECT * FROM Categories where user='" + user.nickname() + "'")

        ids = []

        for cat in categories_all:
            if cat.id not in ids:
                output += '<li><a href="/tabinfo.html?type=session&id=' + str(cat.id) +'">' + cat.name + '</a>\n'
                ids.append(cat.id)

        output += """</ul>
            <!-- Side end -->
        </div>

        <div  class="foot"></div>
    </div>

</div>
<div id="outer2"></div>

<div id="footer">
    &copy; 2009 InterLogic. TabSender server.
</div>
</body></html>
"""
        self.response.out.write(output)
    else:
        self.redirect(users.create_login_url('/tabinfo.html'))
