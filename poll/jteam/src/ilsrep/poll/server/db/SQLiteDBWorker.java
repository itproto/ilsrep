package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollsessionlist;

import java.io.File;
import java.sql.*;

/**
 * This class is for working with SQLite DB.
 * 
 * @author TKOST
 * 
 */
public class SQLiteDBWorker extends DBWorker {

    /**
     * File to load DB from.
     */
    protected String dataFile = "..\\sql\\pollserver.s3db";

    /**
     * Creates <code>SQLiteDBWorker</code> w/o connecting to DB, but stores
     * connection parameters.
     * 
     * @param dataFile
     *            File to load DB from.
     */
    public SQLiteDBWorker(String dataFile) {
        this.dataFile = dataFile;
    }

    /**
     * @see ilsrep.poll.server.db.DBWorker#connect()
     */
    @Override
    public void connect() throws SQLException {
        try { Class.forName("org.sqlite.JDBC");
      Connection conn = DriverManager.getConnection("jdbc:sqlite:"+dataFile);
      this.conn=conn;
    } catch(Exception e) {System.out.println("ExCePTION");}
    }


}
