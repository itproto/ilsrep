package ilsrep.poll.server.db;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Types;
import java.util.ArrayList;
import java.util.List;
import net.sf.xpilotpanel.preferences.Preferences;
import net.sf.xpilotpanel.preferences.model.PreferenceSelector;
import ilsrep.poll.common.Answers;
import org.apache.commons.dbcp.BasicDataSource;

import ilsrep.poll.common.Pollsession;
import ilsrep.poll.common.Poll;
import ilsrep.poll.common.Description;
import ilsrep.poll.common.Choice;
import ilsrep.poll.common.Pollsessionlist;
import ilsrep.poll.common.Item;
import ilsrep.poll.server.PollServer;
import org.apache.log4j.Logger;

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
    private static Logger logger = Logger.getLogger(DBManager.class);

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
    public DBManager(PollServer srvInstance) {
        this.srvInstance = srvInstance;
    }

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
        ResultSet rs = stat
                .executeQuery("select name,testmode,minscore,date from pollsession where id="
                        + id);
        Pollsession sess = null;
        if (rs.next()) {
            // code here
            try {
                sess = new Pollsession();
                sess.setId(id);
                sess.setName(rs.getString("name"));
                sess.setTestMode(rs.getBoolean("testmode") ? "true" : "false");
                sess.setDate(rs.getString("date"));
                if (rs.getBoolean("testmode"))
                    sess.setMinScore(rs.getString("minscore"));
                List<Poll> polls = new ArrayList<Poll>();
                rs = stat
                        .executeQuery("select polls.* from polls left join pollsessions_polls on (polls.id=pollsessions_polls.poll_id) where pollsessions_polls.pollsession_id="
                                + id);
                while (rs.next()) {
                    Poll poll = new Poll();
                    String pollId = rs.getString("id");
                    poll.setId(pollId);
                    poll.setName(rs.getString("name"));
                    Description desc = new Description();
                    desc.setValue(rs.getString("description"));
                    poll.setDescription(desc);
                    poll
                            .setCustomEnabled(rs.getBoolean("customenabled") ? "true"
                                    : "false");
                    // if((rs.getBoolean("customenabled")))
                    // {logger.info("TRUE");}
                    if (sess.getTestMode().equals("true"))
                        poll.setCorrectChoice(rs.getString("correctchoice"));
                    List<Choice> choices = new ArrayList<Choice>();
                    Statement stater2 = conn.createStatement();
                    ResultSet chrs3 = stater2
                            .executeQuery("select choices.* from choices left join polls_choices on (choices.id=polls_choices.choice_id) where polls_choices.poll_id="
                                    + pollId);

                    while (chrs3.next()) {

                        Choice choice = new Choice();
                        choice.setId(chrs3.getString("id"));
                        choice.setName(chrs3.getString("name"));
                        choices.add(choice);
                    }
                    poll.setChoices(choices);
                    polls.add(poll);
                }
                sess.setPolls(polls);

                stat.close();
            }
            catch (Exception e) {
                logger.info("Exception occured while fetching pollsession: "
                        + e.getMessage());
            }
        }
        else {
            sess = null;
        }

        try {
            if (conn != null)
                conn.close();
        }
        catch (SQLException e) {
        }

        return sess;
    }

    public void createUser(String name, String pass) {
        try {
            connect();

            Connection conn = dataSource.getConnection();
            Statement stat = conn.createStatement();
            /* ResultSet rs = */stat
                    .executeQuery("insert into users (userName, password) Values (\""
                            + name + "\",\"" + pass + "\")");
            conn.close();

        }
        catch (Exception e) {
            logger.warn(e.getMessage());
        }

    }

    public String checkUser(String name) {
        try {
            connect();

            Connection conn = dataSource.getConnection();
            Statement stat = conn.createStatement();
            ResultSet rs = stat
                    .executeQuery("select * from users where userName=\""
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
            logger.info(e.getMessage());
            return "false";
        }

    }

    public String authUser(String name, String pass) {
        try {
            connect();

            Connection conn = dataSource.getConnection();
            Statement stat = conn.createStatement();
            ResultSet rs = stat
                    .executeQuery("select * from users where userName=\""
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
            logger.warn(e.getMessage());
            return "false";
        }

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
            Item itm = new Item();
            itm.setId(Integer.toString(rs.getInt("id")));
            itm.setName(rs.getString("name"));
            lstItems.add(itm);
        }
        pollList.setItems(lstItems);

        try {
            stat.close();
        }
        catch (SQLException e) {
            throw e;
        }
        finally {
            try {
                if (conn != null)
                    conn.close();
            }
            catch (SQLException e) {
                throw e;
            }
        }

        return pollList;
    }

    /**
     * Stores pollsession results in DB.
     * 
     * @param ans
     *            Results to store.
     */
    public void saveResults(Answers ans) {
        String id = ans.getPollSesionId();
        String name = ans.getUsername();
        Connection conn = null;
        try {
            conn = dataSource.getConnection();
            conn.setAutoCommit(false);

            PreparedStatement stat = conn
                    .prepareStatement("insert into results (pollsession_id, user_name, poll_id, choice_id, custom_choice, date) VALUES (?, ?, ?, ?, ?, datetime('now'))");
            for (int i = 0; i < ans.getAnswers().size(); i++) {
                stat.setInt(1, Integer.parseInt(id));
                stat.setString(2, name);
                stat.setInt(3, Integer.parseInt(ans.getAnswers().get(i)
                        .getQuestionId()));
                stat.setInt(4, Integer.parseInt(ans.getAnswers().get(i)
                        .getAnswerId()));
                stat.setString(5, ans.getAnswers().get(i).getCustomChoice());
                stat.addBatch();
            }

            stat.executeBatch();

            conn.commit();
        }
        catch (SQLException e) {
        }
        finally {
            try {
                if (conn != null)
                    conn.close();
            }
            catch (SQLException e) {
            }
        }

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
        int i = -1;
        Connection conn = null;
        try {
            conn = dataSource.getConnection();
            conn.setAutoCommit(false);

            // Getting id for next pollsession.
            int pollsessionLastId = -1;
            Statement pollsessionLastIdSt = conn.createStatement();
            ResultSet pollsessionLastIdRs = pollsessionLastIdSt
                    .executeQuery("select id from pollsession order by id desc     limit 1 ");

            if (pollsessionLastIdRs.next()) {
                pollsessionLastId = pollsessionLastIdRs.getInt("id");

                // Inserting new pollsession.
                PreparedStatement pollsessionSt = conn
                        .prepareStatement("insert into pollsession (id, name, testmode, minscore, date) values (?, ?, ?, ?, datetime('now'))");
                pollsessionSt.setInt(1, (pollsessionLastId + 1));
                pollsessionSt.setString(2, sess.getName());

                boolean testmode = sess.getTestMode() != null
                        && sess.getTestMode().compareTo("true") == 0;
                pollsessionSt.setBoolean(3, testmode);
                if (testmode)
                    pollsessionSt.setFloat(4, Float.parseFloat(sess
                            .getMinScore()));
                else
                    pollsessionSt.setNull(4, Types.INTEGER);

                pollsessionSt.executeUpdate();
                pollsessionSt.close();

                i = pollsessionLastId + 1;

                // Processing polls in pollsession.
                for (Poll poll : sess.getPolls()) {
                    // Getting id for next poll.
                    Statement pollsLastIdSt = conn.createStatement();
                    ResultSet pollsLastIdRs = pollsLastIdSt
                            .executeQuery("select id from polls order by id desc     limit 1");

                    int pollsLastId = -1;

                    if (pollsLastIdRs.next()) {
                        pollsLastId = pollsLastIdRs.getInt("id");

                        // Inserting current poll.
                        PreparedStatement pollsSt = conn
                                .prepareStatement("insert into polls (id, name, correctchoice, description, customenabled) values (?, ?, ?, ?, ?)");
                        pollsSt.setInt(1, (pollsLastId + 1));
                        pollsSt.setString(2, poll.getName());
                        pollsSt.setInt(3, Integer.parseInt(poll
                                .getCorrectChoice()));
                        pollsSt.setString(4, poll.getDescription().getValue());
                        pollsSt.setBoolean(5,
                                poll.getCustomEnabled() != null
                                        && poll.getCustomEnabled().compareTo(
                                                "true") == 0);

                        pollsSt.executeUpdate();
                        pollsSt.close();

                        // Updating pollsessions_polls many-to-many linker.
                        Statement pollsessions_pollsSt = conn.createStatement();
                        pollsessions_pollsSt
                                .executeUpdate("insert into pollsessions_polls (pollsession_id, poll_id) values ("
                                        + (pollsessionLastId + 1)
                                        + ", "
                                        + (pollsLastId + 1) + ")");
                        pollsessions_pollsSt.close();

                        int choiceCount = 1;

                        for (Choice choice : poll.getChoices()) {
                            // Getting id for next choice.
                            Statement choicesLastIdSt = conn.createStatement();
                            ResultSet choicesLastIdRs = pollsLastIdSt
                                    .executeQuery("select id from choices order by id desc     limit 1 ");

                            if (choicesLastIdRs.next()) {
                                int choicesLastId = choicesLastIdRs
                                        .getInt("id");

                                // Inserting next choice.
                                PreparedStatement choicesSt = conn
                                        .prepareStatement("insert into choices (id, name) values (?, ?)");
                                choicesSt.setInt(1, (choicesLastId + 1));

                                choicesSt.setString(2, choice.getName());

                                choicesSt.executeUpdate();
                                choicesSt.close();

                                // Updating polls_choices many-to-many linker.

                                Statement polls_choices = conn
                                        .createStatement();
                                polls_choices
                                        .executeUpdate("insert into polls_choices (poll_id, choice_id) values ("
                                                + (pollsLastId + 1)
                                                + ", "
                                                + (choicesLastId + 1) + ")");
                                polls_choices.close();

                                // If testmode, setting correct id(aka real
                                // choice id in db, not relative as editor sent)
                                // for current poll.
                                if (testmode
                                        && choiceCount == Integer.parseInt(poll
                                                .getCorrectChoice())) {
                                    Statement correctChoiceUpdateSt = conn
                                            .createStatement();
                                    correctChoiceUpdateSt
                                            .executeUpdate("UPDATE polls SET correctchoice=\""
                                                    + (choicesLastId + 1)
                                                    + "\" WHERE id=\""
                                                    + (pollsLastId + 1) + "\"");
                                    correctChoiceUpdateSt.close();
                                }

                                choiceCount++;
                            }
                            else
                                i = -1;

                            choicesLastIdSt.close();
                        }

                        pollsLastIdSt.close();
                    }
                    else
                        i = -1;
                }
            }
            else
                i = -1;
            pollsessionLastIdSt.close();

            if (i == -1)
                conn.rollback(); // On errors doing rollback.
            else
                conn.commit(); // Commiting if all proceed correctly.
        }
        catch (SQLException e) {
            logger.info(e.getMessage());
            i = -1;
        }
        finally {
            if (conn != null)
                try {
                    // Closing connection and returning it to pool.
                    if (conn != null)
                        conn.close();
                }
                catch (SQLException e) {
                    i = -1;
                }
        }

        return i;
    }

    /**
     * Removes pollsession from DB.
     * 
     * @param id
     *            Id of pollsession to remove.
     */
    public void removePollsession(String id) {
        Connection conn = null;
        boolean commit = false;

        try {
            conn = dataSource.getConnection();
            conn.setAutoCommit(false);

            // Getting all polls for this pollsession to remove.
            Statement pollsSelectSt = conn.createStatement();
            ResultSet pollsRs = pollsSelectSt
                    .executeQuery("select poll_id from pollsessions_polls where pollsession_id="
                            + id);

            while (pollsRs.next()) {
                commit = true;

                int poll_id = pollsRs.getInt("poll_id");

                // Getting all choices for current poll.
                Statement choicesSelectSt = conn.createStatement();
                ResultSet choicesRs = choicesSelectSt
                        .executeQuery("select choice_id from polls_choices where poll_id="
                                + poll_id);

                while (choicesRs.next()) {
                    int choice_id = choicesRs.getInt("choice_id");

                    // Removing choice from choices table.
                    Statement removeChoiceSt = conn.createStatement();
                    removeChoiceSt
                            .executeUpdate("delete from choices where id="
                                    + choice_id);
                    removeChoiceSt.close();
                }

                // Removing all links to choices for current poll.
                Statement polls_choicesRemoveSt = conn.createStatement();
                polls_choicesRemoveSt
                        .executeUpdate("delete from polls_choices where poll_id="
                                + poll_id);
                polls_choicesRemoveSt.close();

                // Removing current poll.
                Statement pollsRemoveSt = conn.createStatement();
                pollsRemoveSt.executeUpdate("delete from polls where id="
                        + poll_id);
                pollsRemoveSt.close();

                choicesSelectSt.close();
            }

            pollsSelectSt.close();

            // Removing all links to polls for this pollsession.
            Statement pollsessions_pollsRemoveSt = conn.createStatement();
            pollsessions_pollsRemoveSt
                    .executeUpdate("delete from pollsessions_polls where pollsession_id="
                            + id);
            pollsessions_pollsRemoveSt.close();

            // Removing pollsession.
            Statement pollsessionRemoveSt = conn.createStatement();
            pollsessionRemoveSt
                    .executeUpdate("delete from pollsession where id=" + id);
            pollsessionRemoveSt.close();

            if (commit)
                conn.commit();
            else
                conn.rollback();
        }
        catch (SQLException e) {
            e.printStackTrace();
        }
        finally {
            try {
                if (conn != null)
                    conn.close();
            }
            catch (SQLException e) {
            }
        }
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
