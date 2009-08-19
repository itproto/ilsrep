from google.appengine.ext import db

class Categories(db.Model):
    user = db.StringProperty()
    name = db.StringProperty(multiline = False)
    url = db.StringProperty(multiline = False)
    id = db.IntegerProperty()

class Share(db.Model):
    id = db.IntegerProperty()
    user = db.StringProperty()
    sessionid = db.IntegerProperty()
    message = db.StringProperty()
    creator = db.StringProperty()

def deleteSession(id):
    toDelete = db.GqlQuery("SELECT * FROM Categories WHERE id=" + id)
    for cat in toDelete:
        cat.delete();

def deleteURL(id, url):
    toDelete = db.GqlQuery("SELECT * FROM Categories WHERE id=" + id + " and url='" + url + "'")
    for cat in toDelete:
        cat.delete();

def getNewShares(user):
    sharesForUser = db.GqlQuery("SELECT * FROM Share WHERE user='" + user + "'")
    i = 0
    for share in sharesForUser:
        i += 1
    return i

def checkShareExists(target, creator, id):
    sharesForUser = db.GqlQuery("SELECT * FROM Share WHERE user=:1 and creator=:2 and sessionid=:3", target, creator, id)
    for share in sharesForUser:
        return True

    return False

def getNameForID(sessionid):
    search = db.GqlQuery("SELECT * FROM Categories WHERE id=" + str(sessionid))
    for cat in search:
        return cat.name
