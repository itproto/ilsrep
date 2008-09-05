package ilsrep.poll.common.protocol;


import java.util.ArrayList;
import java.util.List;

import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;

/**
 * Testing class, should be removed.
 * 
 * @author TKOST
 * 
 */
public class ProtocolTester {

    public static void main(String[] args) throws JAXBException {
        for (Pollpacket packet : getTestPackets()) {
            printPacket(packet);
            System.out.println();
        }
    }

    public static void printPacket(Pollpacket packet) throws JAXBException {
        JAXBContext pollpacketCont = JAXBContext.newInstance(Pollpacket.class);

        Marshaller mr = pollpacketCont.createMarshaller();
        mr.setProperty("jaxb.formatted.output", true);
        mr.marshal(packet, System.out);
    }

    private static List<Pollpacket> getTestPackets() {
        List<Pollpacket> packets = new ArrayList<Pollpacket>();

        {
            Pollpacket packet0 = new Pollpacket();

            Request req = new Request();
            req.setType(Request.TYPE_LIST);

            packet0.setRequest(req);

            packets.add(packet0);
        }

        {
            Pollpacket packet1 = new Pollpacket();

            Request req = new Request();
            req.setType(Request.TYPE_POLLXML);
            req.setId("666");

            packet1.setRequest(req);

            packets.add(packet1);
        }

        {
            Pollpacket packet2 = new Pollpacket();

            Pollsessionlist lst = new Pollsessionlist();

            List<Item> lstItems = new ArrayList<Item>();

            Item i0 = new Item();
            i0.setId("13");
            i0.setName("thirteen");
            lstItems.add(i0);

            Item i1 = new Item();
            i1.setId("666");
            i1.setName("six six six");
            lstItems.add(i1);

            lst.setItems(lstItems);

            packet2.setPollsessionList(lst);

            packets.add(packet2);
        }

        return packets;
    }

}
