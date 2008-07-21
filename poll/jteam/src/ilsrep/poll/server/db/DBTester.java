package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollpacket;
import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.ProtocolTester;

import java.io.StringReader;
import java.sql.SQLException;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Unmarshaller;

/**
 * Testing class, should be removed.
 * 
 * @author TKOST
 * 
 */
public class DBTester {

    public static void main(String[] args) throws ClassNotFoundException,
            SQLException, JAXBException {
        DBWorker db = new SQLiteDBWorker(null, "sql/pollserver.s3db");

        db.connect();

        // printSelects(db);

        String someXml = db.getPollsessionById("1");
        StringReader xmlReader = new StringReader(someXml);
        JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession sess = (Pollsession) um.unmarshal(xmlReader);
        System.out.println(db.storePollsession(sess));

        printSelects(db);

        db.close();
    }

    private static void printSelects(DBWorker db) throws SQLException,
            JAXBException {
        Pollsessionlist lst = db.getPollsessionlist();

        Pollpacket packet = new Pollpacket();
        packet.setPollsessionList(lst);

        ProtocolTester.printPacket(packet);

        final char newLine = '\n';
        System.out.println(newLine + db.getPollsessionById("1") + newLine
                + db.getPollsessionById("2") + newLine
                + db.getPollsessionById("3"));
    }

}
