package ilsrep.poll.server.db;

import ilsrep.poll.server.PollServer;

/**
 * This class is for working with SQLite DB.
 * 
 * @author TKOST
 * @author DRC
 * 
 */
public class SQLiteDBWorker extends DBWorker {

    /**
     * File to load DB from.
     */
    protected String dataFile = "..\\sql\\polls3.s3db";

    /**
     * SQLite's JDBC driver name.
     */
    public static final String SQLite_DRIVER_CLASS_NAME = "org.sqlite.JDBC";

    /**
     * Creates <code>SQLiteDBWorker</code> w/o connecting to DB, but stores
     * connection parameters.
     * 
     * @param dataFile
     *            File to load DB from.
     * @throws ClassNotFoundException
     *             If DB driver not found.
     */
    public SQLiteDBWorker(PollServer srvInstance, String dataFile)
            throws ClassNotFoundException {
        super(srvInstance);

        this.dataFile = dataFile;

        Class.forName(SQLite_DRIVER_CLASS_NAME);
        driverClassName = SQLite_DRIVER_CLASS_NAME;
        dbURL = "jdbc:sqlite:" + dataFile;
    }

}
