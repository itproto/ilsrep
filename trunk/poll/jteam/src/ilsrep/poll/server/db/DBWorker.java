package ilsrep.poll.server.db;

import ilsrep.poll.common.Pollsessionlist;

import java.sql.SQLException;

/**
 * This abstract class is utility for working with any DB.<br>
 * There should be concrete class, which works with concrete DB.
 * 
 * @author TKOST
 * 
 */
public abstract class DBWorker {

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
    public abstract String getPollsessionById(String id) throws SQLException;

    /**
     * Fetches pollsession list from DB.
     * 
     * @return Pollsession list in object representation.
     * @throws SQLException
     *             On some DB problems.
     */
    public abstract Pollsessionlist getPollsessionlist() throws SQLException;

}
