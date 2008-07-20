package ilsrep.poll.server.db;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import javax.sql.DataSource;

import net.sf.xpilotpanel.preferences.Preferences;
import net.sf.xpilotpanel.preferences.model.PreferenceSelector;

import org.apache.commons.dbcp.BasicDataSource;

import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.Item;
import ilsrep.poll.server.PollServer;

/**
 * This abstract class is utility for working with any DB.<br>
 * There should be concrete class, which works with concrete DB.
 * 
 * @author TKOST
 * @author DRC
 * 
 */
public abstract class DBWorker {

    /**
     * Default minimal number of idle connections in pool.
     */
    public int DEFAULT_MIN_IDLE = 1;

    /**
     * Default initial quantity of connections in pool.
     */
    public int DEFAULT_INITIAL_SIZE = 1;

    /**
     * Default maximal number of idle connections in pool.
     */
    public int DEFAULT_MAX_IDLE = 4;

    /**
     * Default minimal number of active connections in pool.
     */
    public int DEFAULT_MAX_ACTIVE = 16;

    /**
     * <code>DataSource</code> for DB.<br>
     * Is pool.
     */
    protected DataSource dataSource = null;

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
    protected PollServer srvInstance = null;

    /**
     * Server instance to read configuration from.
     * 
     * @param srvInstance
     *            Server instance.
     */
    public DBWorker(PollServer srvInstance) {
        this.srvInstance = srvInstance;
    }

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
            if (srvInstance != null) {
                Preferences conf = srvInstance.getConfiguration();
                dbPool.setMinIdle(readIntOptionFromConfiguration(conf,
                        "poolMinIdle"));
                dbPool.setInitialSize(readIntOptionFromConfiguration(conf,
                        "poolInitialSize"));
                dbPool.setMaxIdle(readIntOptionFromConfiguration(conf,
                        "poolMaxIdle"));
                dbPool.setMaxActive(readIntOptionFromConfiguration(conf,
                        "poolMaxActive"));
            }
            else {
                dbPool.setMinIdle(DEFAULT_MIN_IDLE);
                dbPool.setInitialSize(DEFAULT_INITIAL_SIZE);
                dbPool.setMaxIdle(DEFAULT_MAX_IDLE);
                dbPool.setMaxActive(DEFAULT_MAX_ACTIVE);
            }
        }
    }

    /**
     * Fetches pollsession from DB by id.
     * 
     * @param id
     *            Id of pollsession to fetch.
     * @return Pollsession as string or null if pollsession not found.
     * @throws SQLException
     *             On some DB problems.
     */
    public String getPollsessionById(String id) throws SQLException {
        connect(); // If connection isn't established yet this connects to DB.

        Connection conn = dataSource.getConnection();
        Statement stat = conn.createStatement();

        String xmlItself;
        ResultSet rs = stat
                .executeQuery("select xml from polls where id=" + id);
        if (rs.next()) {
            xmlItself = rs.getString("xml");
        }
        else {
            xmlItself = null;
        }

        stat.close();
        conn.close();

        return xmlItself;
    }

    /**
     * Fetches pollsession list from DB.
     * 
     * @return Pollsession list in object representation.
     * @throws SQLException
     *             On some DB problems.
     */
    public Pollsessionlist getPollsessionlist() throws SQLException {
        connect(); // If connection isn't established yet this connects to DB.

        Connection conn = dataSource.getConnection();
        Statement stat = conn.createStatement();

        Pollsessionlist pollList = new Pollsessionlist();
        List<Item> lstItems = new ArrayList<Item>();
        ResultSet rs = stat.executeQuery("select id, name from polls");
        while (rs.next()) {
            Item itm = new Item();
            itm.setId(Integer.toString(rs.getInt("id")));
            itm.setName(rs.getString("name"));
            lstItems.add(itm);
        }
        pollList.setItems(lstItems);

        stat.close();
        conn.close();

        return pollList;
    }

    /**
     * `Cos of XPilotPanel lib's functionality lack this is used to read "int"
     * option and if option in configuration file is corrupted use default value
     * from <code>PreferencesModel</code>.
     * 
     * @param conf
     *            Configuration.
     * @param option
     *            Option name.
     * 
     * @return Option as int.
     */
    public static int readIntOptionFromConfiguration(Preferences conf,
            String option) {
        int optionInt = -1;

        try {
            optionInt = Integer.parseInt(conf.get(option));
        }
        catch (NumberFormatException e) {
            PreferenceSelector sel = new PreferenceSelector();
            sel.setName(option);
            optionInt = Integer.parseInt(conf.getModel()
                    .getPreferenceBySelector(sel).getDefaultValue());
        }
        catch (NullPointerException e) {
            optionInt = -1;
        }

        return optionInt;
    }

}
