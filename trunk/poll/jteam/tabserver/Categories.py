from google.appengine.ext import db

class Categories(db.Model):
  user = db.StringProperty()
  name = db.StringProperty(multiline=False)
  url = db.StringProperty(multiline=False)
  id = db.IntegerProperty()
