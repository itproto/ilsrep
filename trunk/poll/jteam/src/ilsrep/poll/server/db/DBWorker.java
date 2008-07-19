package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.Item;
import java.sql.*;
import java.util.*;

/**
 * This abstract class is utility for working with any DB.<br>
 * There should be concrete class, which works with concrete DB.
 * 
 * @author TKOST
 * 
 */
public abstract class DBWorker {
public Connection conn;
    /**
     * Establishes connection to concrete(should be overriden) DB.
     * 
     * @throws SQLException
     *             On some DB problems.
     */
    public abstract void connect() throws SQLException;

    /**
     * Fetches pollsession from DB by id.
     * 
     * @param id
     *            Id of pollsession to fetch.
     * @return Pollsession as string.
     * @throws SQLException
     *             On some DB problems.
     */
    public  String getPollsessionById(String id) throws SQLException{
	    this.connect();
	    String xmlItself;
	   Statement stat = this.conn.createStatement(); 
	   ResultSet rs = stat.executeQuery("select xml from polls where id="+id);
	   if (rs.next()) {xmlItself=rs.getString("xml") ;
	   }   else {
		   xmlItself=null; }
		   this.conn.close();
		   return xmlItself;
	    
	    
	    
	    };

    /**
     * Fetches pollsession list from DB.
     * 
     * @return Pollsession list in object representation.
     * @throws SQLException
     *             On some DB problems.
     */
    public Pollsessionlist  getPollsessionlist() throws SQLException{
	    this.connect();
	        Pollsessionlist  pollList=new Pollsessionlist();
	        List<Item> lstItems = new ArrayList<Item>();
Item itm = new Item();
	   Statement stat = this.conn.createStatement(); 
	   ResultSet rs = stat.executeQuery("select id, name from polls");
	   while (rs.next()) {
		   itm.setId(Integer.toString(rs.getInt("id")));
		   itm.setName(rs.getString("name"));
		   lstItems.add(itm);
		   	   }   ;
		   	   pollList.setItems(lstItems);
	   this.conn.close();
	    return pollList;
	    
	    
	    };
	    
	    
	    
	    };

