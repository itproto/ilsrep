from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

import Categories

class TabInfoPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    user = users.get_current_user()

    if user:
        output = """<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>

<head>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />
<title>TabServer Sessions Information</title>
<meta name="keywords" content="tabsender, ils, jteam" />
<meta name="description" content="jteam tabsender" />
</head>

<body>
"""

        type = self.request.get('type')

        if (not type) or (type == 'user'):
            output += '<h1>Your(' + user.nickname() + ') stored sessions on server:</h1><ul>'

            categories_all = db.GqlQuery("SELECT * FROM Categories where user='" + user.nickname() + "'")

            ids = []

            for cat in categories_all:
                if cat.id not in ids:
                    output += '<li><a href="/tabinfo.html?type=session&id=' + str(cat.id) +'">' + cat.name + '</a></li>'
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
                    output += '<h1>URLs for session "' + name + '":</h1><ul>'
                    for link in links:
                        output += '<li><a href="' + link + '">' + link + '</a></li>'
                    output += '</ul>'
                else:
                    output += 'No links for this session!'

        output += '</body></html>'

        self.response.out.write(output)
    else:
        self.redirect(users.create_login_url('/tabinfo.html'))
