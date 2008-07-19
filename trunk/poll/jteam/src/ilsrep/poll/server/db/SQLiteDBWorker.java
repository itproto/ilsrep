package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollsessionlist;

import java.io.File;
import java.sql.SQLException;

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
    protected File dataFile = null;

    /**
     * Creates <code>SQLiteDBWorker</code> w/o connecting to DB, but stores
     * connection parameters.
     * 
     * @param dataFile
     *            File to load DB from.
     */
    public SQLiteDBWorker(File dataFile) {
        this.dataFile = dataFile;
    }

    /**
     * @see ilsrep.poll.server.db.DBWorker#connect()
     */
    @Override
    public void connect() throws SQLException {
        // TODO: Fix connection to SQLLite.
    }

    /**
     * @see ilsrep.poll.server.db.DBWorker#getPollsessionById(java.lang.String)
     */
    @Override
    public String getPollsessionById(String id) throws SQLException {
        // TODO: Fix getting pollsession by id from SQLLite.
        return null;
    }

    /**
     * @see ilsrep.poll.server.db.DBWorker#getPollsessionlist()
     */
    @Override
    public Pollsessionlist getPollsessionlist() throws SQLException {
        // TODO: Fix getting pollsession list from SQLLite.
        return null;
    }

}
