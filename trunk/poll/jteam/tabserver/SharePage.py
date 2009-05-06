from google.appengine.ext import webapp
from google.appengine.ext import db
from google.appengine.api import users

from Categories import Categories
from Categories import deleteSession
from Categories import deleteURL
from Categories import getNewShares
from Categories import getNameForID

from HtmlUtils import HtmlUtils

from urllib import quote_plus

class SharePage(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/html'

    user = users.get_current_user()

    if user:
        htmlUtils = HtmlUtils()

        output = htmlUtils.generateStart("TabSender: My account")
        output += htmlUtils.generateDefaultMenu()
        output += htmlUtils.generateMenuEnd()

        newShares = getNewShares(user.nickname())

        type = self.request.get('type')

        if not type or type == 'browse':
            output += '<h1><span class="colouredText">' + user.nickname() + '</span>\'s shares on server:</h1><ul>'
            if newShares == 0:
                output += 'No new shares.<br />Return to <a href="/tabinfo.html">your account</a>.'
            else:
                output += '<ul>\n'
                sharesForUser = db.GqlQuery("SELECT * FROM Share WHERE user='" + user.nickname() + "'")
                for share in sharesForUser:
                    output += '<li><a href="/share.html?type=details&id=' + str(share.id) + '">' + getNameForID(share.sessionid) + '</a> by <span class="colouredText">' + share.creator + '</span><br /><span class="colouredText">Message</span>: ' + share.message + '<br />\n'
                    output += '<a href="/share.html?type=accept&id=' + str(share.id) + '"><img class="imageURL" src="/html/images/add.png" />Accept</a>\n'
                    output += '<a href="/share.html?type=cancel&id=' + str(share.id) + '"><img class="imageURL" src="/html/images/cancel.png" />Cancel</a>\n'
                    output += '</li>\n'
                output += '</ul>\n'
#        if type == 'browsedb':
#            sharesTable = db.GqlQuery("SELECT * FROM Share")
#            for share in sharesTable:
#                output += str(share.user) + ' ' + str(share.sessionid) + ' ' + str(share.message) + ' ' + str(share.creator) + '<br />'
        elif type == 'cancel':
            id = self.request.get('id')
            shareToCancel = db.GqlQuery("SELECT * FROM Share WHERE id=:1", int(id))
            for share in shareToCancel:
                share.delete()
            self.redirect('/share.html')

        output += htmlUtils.generateMainPartEnd()

#        if newShares == 0:
#            output += 'You have no new shares.'
#        else:
#            output += 'TO FIX!!!'

        output += htmlUtils.generateEnd()

        self.response.out.write(output)
    else:
        self.redirect(users.create_login_url('/share.html'))
