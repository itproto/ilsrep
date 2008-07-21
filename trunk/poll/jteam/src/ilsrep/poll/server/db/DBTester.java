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

        // writeXml(db, "1");

        printSelects(db);

        db.close();
    }

    public static void printSelects(DBWorker db) throws SQLException,
            JAXBException {
        Pollsessionlist lst = db.getPollsessionlist();

        Pollpacket packet = new Pollpacket();
        packet.setPollsessionList(lst);

        ProtocolTester.printPacket(packet);

        final char newLine = '\n';
        for (int i = 1; i < 10; i++) {
            System.out.println(newLine
                    + db.getPollsessionById(Integer.toString(i)) + newLine);
        }
    }

    public static void writeXml(DBWorker db, String alreadyExistingID)
            throws SQLException, JAXBException {
        String someXml = db.getPollsessionById(alreadyExistingID);
        StringReader xmlReader = new StringReader(someXml);
        JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
        Unmarshaller um = cont.createUnmarshaller();
        Pollsession sess = (Pollsession) um.unmarshal(xmlReader);
        db.storePollsession(sess);
    }

}
