package ilsrep.poll.server.db;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;
import javax.xml.bind.JAXBContext;
import javax.xml.bind.JAXBException;
import javax.xml.bind.Marshaller;
import java.io.StringWriter;
import org.apache.log4j.Logger;
import net.sf.xpilotpanel.preferences.Preferences;
import net.sf.xpilotpanel.preferences.model.PreferenceSelector;

import org.apache.commons.dbcp.BasicDataSource;

import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Description;
import ilsrep.poll.common.Choice;
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

    private static Logger logger = Logger.getLogger(DBWorker.class);

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
    public Pollsession getPollsessionById(String id) throws SQLException {
        connect(); // If connection isn't established yet this connects to DB.

        Connection conn = dataSource.getConnection();
        Statement stat = conn.createStatement();
        logger.info("Started bullshit");
        ResultSet rs = stat
                .executeQuery("select name,testmode,minscore from pollsession where id="
                        + id);
        Pollsession sess = null;
        if (rs.next()) {
            // code here
            try {
                logger.info("Started bullshit2");
                sess = new Pollsession();
                sess.setId(id);
                sess.setName(rs.getString("name"));
                sess.setTestMode(rs.getBoolean("testmode") ? "true" : "false");
                if (rs.getBoolean("testmode"))
                    sess.setMinScore(rs.getString("minscore"));
                logger.info(sess.getId() + sess.getName());
                List<Poll> polls = new ArrayList<Poll>();
                rs = stat
                        .executeQuery("select poll_id from pollsessions_polls where pollsession_id="
                                + id);
                int pollnum = 0;
                while (rs.next()) {
                    pollnum++;
                    Poll poll = new Poll();
                    String pollId = rs.getString("poll_id");
                    poll.setId(Integer.toString(pollnum));
                    Statement stater = conn.createStatement();
                    ResultSet chrs = stater
                            .executeQuery("select * from polls where id="
                                    + pollId);
                    poll.setName(chrs.getString("name"));
                    Description desc = new Description();
                    desc.setValue(chrs.getString("description"));
                    poll.setDescription(desc);
                    poll
                            .setCustomEnabled(chrs.getBoolean("customenabled") ? "true"
                                    : "false");
                    if (sess.getTestMode().equals("true"))
                        poll.setCorrectChoice(chrs.getString("correctchoice"));
                    logger.info(poll.getId() + poll.getName() + pollId);
                    List<Choice> choices = new ArrayList<Choice>();
                    Statement stater2 = conn.createStatement();
                    ResultSet chrs3 = stater2
                            .executeQuery("select choice_id from polls_choices where poll_id="
                                    + pollId);
                    int choicenum = 0;

                    while (chrs3.next()) {
                        logger.info("were in");
                        choicenum++;
                        String choiceId = chrs3.getString("choice_id");
                        logger.info("CYCLE!");
                        Choice choice = new Choice();
                        choice.setId(choiceId);
                        logger.info("select * from choices where id="
                                + choiceId);
                        logger.info("CYCLE!2");
                        logger.info("select * from choices where id="
                                + choiceId);
                        Statement stat2 = conn.createStatement();
                        ResultSet chrs2 = stat2
                                .executeQuery("select * from choices where id="
                                        + choiceId);
                        logger.info("CYCLE!3");
                        choice.setName(chrs2.getString("name"));
                        logger.info("CYCLE!4");
                        logger.info(choice.getId() + choice.getName());
                        logger.info("CYCLE!5");
                        choices.add(choice);
                    }
                    poll.setChoices(choices);
                    polls.add(poll);
                }
                sess.setPolls(polls);
            }
            catch (Exception e) {
                logger.info(e.getMessage());
            }
        }
        else {
            sess = null;
        }

        stat.close();
        conn.close();

        return sess;
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
        ResultSet rs = stat.executeQuery("select id, name from pollsession");
        while (rs.next()) {
            logger.info(rs.getString("name"));
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

    /**
     * Stores pollsession in DB.
     * 
     * @param sess
     *            Pollsession, w/o id.
     * @return Id of this pollsession in DB, or <code>-1</code> if it weren't
     *         added.
     */
    public int storePollsession(Pollsession sess) {
        int i = 0;
        try {
            Connection conn = dataSource.getConnection();
            Statement stat = conn.createStatement();

            // Calculating id for this pollsession.
            ResultSet rs = stat.executeQuery("select id from polls");
            List<String> list = new ArrayList<String>();
            while (rs.next())
                list.add(rs.getString("id"));
            while (true) {
                i++;
                if (!list.contains(Integer.toString(i)))
                    break;
            }
            sess.setId(Integer.toString(i));

            // Preparing pollsession for writing into DB(marshalling from object
            // into stream).
            JAXBContext cont = JAXBContext.newInstance(Pollsession.class);
            Marshaller m = cont.createMarshaller();
            StringWriter os = new StringWriter();
            m.marshal(sess, os);
            m.setProperty("jaxb.formatted.output", true);

            // Writing pollsession into DB.
            stat.executeUpdate("insert into polls(id, name, xml) values ("
                    + Integer.toString(i) + ",\"" + sess.getName() + "\",\'"
                    + os.toString() + "\')");

            stat.close();
            conn.close();
        }
        catch (SQLException e) {
            i = -1;
        }
        catch (JAXBException e) {
            i = -1;
        }

        return i;
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
    @Override
    protected void finalize() throws Throwable {
        super.finalize();
        close();
    }

}
