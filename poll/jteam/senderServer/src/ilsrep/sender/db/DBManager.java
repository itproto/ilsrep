package ilsrep.sender.db;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;
import org.apache.commons.dbcp.BasicDataSource;

import ilsrep.sender.*;



/**
 * This abstract class is utility for working with any DB.<br>
 * There should be concrete class, which works with concrete DB.
 * 
 * @author TKOST
 * @author DRC
 * 
 */

public abstract class DBManager {

    /**
     * Log4j Logger for this class.
     */
    // private static Logger logger = Logger.getLogger(DBManager.class);
    /**
     * Default minimal number of idle connections in pool.
     */
    public int DEFAULT_MIN_IDLE = 5;

    /**
     * Default initial quantity of connections in pool.
     */
    public int DEFAULT_INITIAL_SIZE = 1;

    /**
     * Default maximal number of idle connections in pool.
     */
    public int DEFAULT_MAX_IDLE = 20;

    /**
     * Default minimal number of active connections in pool.
     */
    public int DEFAULT_MAX_ACTIVE = 256;

    /**
     * <code>DataSource</code> for DB.<br>
     * Is pool.
     */
    protected BasicDataSource dataSource = null;

    /**
     * DB's URL for JDBC.
     */
    protected String dbURL = null;

    /**
     * DB driver's class name.
     */
    protected String driverClassName = null;

    /**
     * Server instance(to read configuration from).
     */
  

    /**
     * Routine fix, due to featureless SQLite...
     */
    public static final ThreadLocal<Connection> threadLocalConnection = new ThreadLocal<Connection>();

    /**
     * Server instance to read configuration from.
     * 
     * @param srvInstance
     *            Server instance.
     */
  
    // private static Logger logger = Logger.getLogger(DBWorker.class);

    /**
     * Initialises connection pool to concrete(should be overridden) DB.
     * 
     * @throws SQLException
     *             On some DB problems.
     */
    public void connect() throws SQLException {
        if (dataSource == null) {
            BasicDataSource dbPool = new BasicDataSource();
            dbPool.setDriverClassName(driverClassName);
            dbPool.setUrl(dbURL);

            dataSource = dbPool;

            // Reading pool's configuration.
            
            
                dbPool.setMinIdle(DEFAULT_MIN_IDLE);
                dbPool.setInitialSize(1);
                dbPool.setMaxIdle(DEFAULT_MAX_IDLE);
                dbPool.setMaxActive(DEFAULT_MAX_ACTIVE);
                   }
    }

  

    public void createUser(String name, String pass) throws Exception{
        
            connect();
            Connection conn = null;
            if (threadLocalConnection.get() == null)
                conn = dataSource.getConnection();
            else
                conn = threadLocalConnection.get();
            Statement stat = conn.createStatement();
            stat.executeUpdate("insert into users (username, password) Values (\""
                            + name + "\",\"" + pass + "\")");
            conn.close();

        
        

    }

    public String checkUser(String name) {
        try {
            connect();

            Connection conn = null;
            if (threadLocalConnection.get() == null)
                conn = dataSource.getConnection();
            else
                conn = threadLocalConnection.get();
            Statement stat = conn.createStatement();
            ResultSet rs = stat
                    .executeQuery("select * from users where username=\""
                            + name + "\"");
            if (rs.next()) { // logger.info("User Exists");
                conn.close();
                return "true";

            }
            else {// logger.info("No such user");
                conn.close();
                return "false";

            }
        }
        catch (Exception e) {
            // logger.info(e.getMessage());
            return "false";
        }

    }

    public String authUser(String name, String pass) {
        try {
            connect();

            Connection conn = null;
            if (threadLocalConnection.get() == null)
                conn = dataSource.getConnection();
            else
                conn = threadLocalConnection.get();
            Statement stat = conn.createStatement();
            ResultSet rs = stat
                    .executeQuery("select * from users where username=\""
                            + name + "\" and password=\"" + pass + "\"");
            if (rs.next()) { // logger.info("User Logged");
                conn.close();
                return "true";
            }
            else {// logger.info("Wrong pass");
                conn.close();
                return "false";

            }
        }
        catch (Exception e) {
            // logger.warn(e.getMessage());
            return "false";
        }

    }

    
    public Connection getConnection() throws SQLException {
        connect();

        return dataSource.getConnection();
    }

    /**
     * Stops connections to DB.
     * 
     * @throws SQLException
     *             On some DB errors.
     */
   public void close() throws SQLException {
        dataSource.close();
    }

    /**
     * @see java.lang.Object#finalize()
     */
    /*@Override
    protected void finalize() throws Throwable {
        super.finalize();
        close();
    }
    */

}
