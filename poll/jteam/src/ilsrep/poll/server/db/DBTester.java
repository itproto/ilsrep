package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollpacket;
import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.ProtocolTester;

import java.sql.SQLException;

import javax.xml.bind.JAXBException;

/**
 * Testing class, should be removed.
 * 
 * @author TKOST
 * 
 */
public class DBTester {

    public static void main(String[] args) throws ClassNotFoundException,
            SQLException, JAXBException {
        DBWorker db = new SQLiteDBWorker("sql/pollserver.s3db");

        db.connect();

        Pollsessionlist lst = db.getPollsessionlist();

        Pollpacket packet = new Pollpacket();
        packet.setPollsessionList(lst);

        ProtocolTester.printPacket(packet);

        final char newLine = '\n';
        System.out.println(newLine + db.getPollsessionById("1") + newLine
                + db.getPollsessionById("2") + newLine
                + db.getPollsessionById("666"));
    }

}
