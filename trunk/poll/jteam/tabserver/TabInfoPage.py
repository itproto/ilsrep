from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

from Categories import Categories
from Categories import deleteSession
from Categories import deleteURL

from HtmlUtils import HtmlUtils

from urllib import quote_plus

class TabInfoPage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    user = users.get_current_user()

    if user:
        htmlUtils = HtmlUtils()

        output = htmlUtils.generateStart("TabSender: My account")
        output += htmlUtils.generateDefaultMenu()
        output += htmlUtils.generateMenuEnd()

        type = self.request.get('type')

        if (not type) or (type == 'user'):
            output += '<h1><span class="colouredText">' + user.nickname() + '</span>\'s stored sessions on server:</h1><ul>'

            categories_all = db.GqlQuery("SELECT * FROM Categories where user='" + user.nickname() + "'")

            ids = []
            output1 = ""

            for cat in categories_all:
                if cat.id not in ids:
                    output1 += '<li><a href="/tabinfo.html?type=session&id=' + str(cat.id) + '">' + cat.name + '</a>\n'
                    output1 += '<a href="/tabinfo.html?type=removesession&id=' + str(cat.id) + '"><img class="imageURL" src="/html/images/cross.png" /></a>' + '</li>\n'
#                    output += '[' + '<a href="/tabinfo.html?type=removesession&id='+ str(cat.id) + '">Delete</a>' + ']</li>\n'
                    ids.append(cat.id)

            if len(ids) > 0:
                output += "<ul>\n" + output1 + "</ul>\n"
            else:
                output += "<h2>Session list is empty!</h2>\n"

            output += '<br />\n<a href="/tabinfo.html?type=newsession"><img class="imageURL" src="/html/images/add.png" />New session</a>\n'
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
                        output += '<a href="/tabinfo.html?type=removeurl&id=' + str(id) + '&url=' + quote_plus(link) + '"><img class="imageURL" src="/html/images/cross.png" /></a>' + '</li>\n'
#                        output += '[' + '<a href="/tabinfo.html?type=removeurl&id='+ str(id) + '&url=' + quote_plus(link) + '">Delete</a>' + ']</li>\n'
                    output += '</ul>'

                    output += '<form method="get" action="/tabinfo.html">' + '\n'
                    output += '<input type="hidden" name="type" value="posturl" />' + '\n'
                    output += '<input type="hidden" name="id" value="' + id + '" />' + '\n'
                    output += '<input type="text" size="30" name="url" />' + '\n'
                    output += '<input type="submit" value="Add URL" /> </form>' + '\n'
                else:
                    output += '<h2>Session was deleted!</h2> '

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

                name_select = db.GqlQuery("SELECT * FROM Categories WHERE id=" + id + " LIMIT 1")
                cat.name = name_select[0].name
                cat.id = name_select[0].id

                cat.put()

                self.redirect("/tabinfo.html?type=session&id=" + id)
        elif type == "newsession":
            output += '<form method="get" action="/tabinfo.html">' + '\n'
            output += '<input type="hidden" name="type" value="newsessioncreate" />' + '\n'
            output += '<p>Session name:</p>'
            output += '<input type="text" size="30" name="sessionName" />' + '\n'
            output += '<p>First URL:</p>'
            output += '<input type="text" size="30" name="url" />' + '\n'
            output += '<input type="submit" value="Add session" /> </form>' + '\n'
        elif type == "newsessioncreate":
            sessionName = self.request.get('sessionName')
            url = self.request.get('url')
            
            if sessionName and url:
                categories_all = db.GqlQuery("SELECT * FROM Categories ORDER BY id DESC LIMIT 1")
                curid = 0
                for cat in categories_all:
                   curid = cat.id + 1;
    
                newSession = Categories()
                newSession.id = curid
                newSession.user = user.nickname()
                newSession.name = sessionName
                newSession.url = url
                newSession.put()

            self.redirect("/tabinfo.html")

        output += htmlUtils.generateMainPartEnd()

        output += "<ul>"

        categories_all = db.GqlQuery("SELECT * FROM Categories where user='" + user.nickname() + "'")

        ids = []

        i = 0
        lastI = 0
        moreWritten = False

        for cat in categories_all:
            if cat.id not in ids:
                if i < 13:
                    output += '<li><a href="/tabinfo.html?type=session&id=' + str(cat.id) + '">' + cat.name + '</a>\n'
                elif not moreWritten:
                    moreWritten = True
                    lastI = i
                ids.append(cat.id)
                i += 1

        if moreWritten:
            output += '<li>And ' + str(len(ids) - lastI) + ' more...</li>'

        output += "</ul>"
        output += htmlUtils.generateEnd()

        self.response.out.write(output)
    else:
        self.redirect(users.create_login_url('/tabinfo.html'))
