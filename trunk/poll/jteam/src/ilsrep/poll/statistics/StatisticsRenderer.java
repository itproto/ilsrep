package ilsrep.poll.statistics;

import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.server.db.DBManager;

import java.awt.Dimension;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PiePlot;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.data.category.DefaultCategoryDataset;
import org.jfree.data.general.DefaultKeyedValuesDataset;

/**
 * @author TKOST
 */
public class StatisticsRenderer {

    /**
     * Database to read statistics from.
     */
    protected DBManager db = null;

    /**
     * Dimension of rendered charts.
     */
    protected Dimension dimension = null;

    /**
     * Creates statistics renderer.
     * 
     * @param db
     *            Database to read statistics from.
     * @param dimension
     *            Dimension of rendered charts.
     */
    public StatisticsRenderer(DBManager db, Dimension dimension) {
        this.db = db;
        this.dimension = dimension;
    }

    /**
     * Renders statistics chart(of given type).
     * 
     * @param type
     *            Type of chart.
     * @return Rendered statistics chart.
     */
    public JFreeChart renderStatisticsChart(StatisticsType type) {
        JFreeChart renderedChart = null;

        switch (type) {
            case SURVEYS_TOTAL_SUCCESS_FAIL_STATS:
                renderedChart = renderSurveysTotalSuccessFailStats();
                break;
            case POLLS_TOTAL_SUCCESS_FAIL_STATS:
                renderedChart = renderPollsTotalSuccessFailStats();
                break;
            case POLLS_WITH_CUSTOM_CHOICE:
                renderedChart = renderPollsWithCustomChoice();
                break;
        }

        return renderedChart;
    }

    /**
     * @see StatisticsType#SURVEYS_TOTAL_SUCCESS_FAIL_STATS
     */
    protected JFreeChart renderSurveysTotalSuccessFailStats() {
        // Due to bugs in DBManager and in commented out code below this chart
        // is disabled.
        // TODO: Fix these bugs.
        return null;

        // int successSurveys = 0;
        // int failSurveys = 0;
        //
        // Connection dbConn = null;
        // try {
        // dbConn = db.getConnection();
        //
        // Statement surveysSt = dbConn.createStatement();
        // ResultSet surveysRs = surveysSt
        // .executeQuery("select id from pollsession");
        //
        // while (surveysRs.next()) {
        // int surveyId = surveysRs.getInt("id");
        //
        // DBManager.threadLocalConnection.set(dbConn);
        // Pollsession currentSurvey = db.getPollsessionById(Integer
        // .toString(surveyId));
        // DBManager.threadLocalConnection.set(null);
        //
        // Statement surveySeancesSt = dbConn.createStatement();
        // ResultSet surveySeancesRs = surveySeancesSt
        // .executeQuery(
        // "select distinct date from results where pollsession_id = "
        // + surveyId);
        //
        // while (surveySeancesRs.next()) {
        // Timestamp currentSeanceDate = surveySeancesRs
        // .getTimestamp(1);
        //
        // System.out.println(currentSeanceDate.toString());
        //
        // int successPolls = 0;
        //
        // for (Poll poll : currentSurvey.getPolls()) {
        // Statement pollResultSt = dbConn.createStatement();
        // ResultSet pollResultRs = pollResultSt
        // .executeQuery("select choice_id from results where poll_id = "
        // + poll.getId()
        // + " and\"date = "
        // + currentSeanceDate.toString() + "\"");
        //
        // if (pollResultRs.next()
        // && Integer.toString(
        // pollResultRs.getInt("choice_id"))
        // .equals(poll.getCorrectChoice()))
        // successPolls++;
        //
        // pollResultSt.close();
        // }
        //
        // if (((double) successPolls)
        // / currentSurvey.getPolls().size() > Integer
        // .parseInt(currentSurvey.getMinScore()))
        // successSurveys++;
        // else
        // failSurveys++;
        // }
        // }
        //
        // System.out.println(successSurveys + " " + failSurveys);
        // }
        // catch (SQLException e) {
        // e.printStackTrace();
        // successSurveys = failSurveys = -1;
        // }
        // finally {
        // try {
        // dbConn.close();
        // }
        // catch (Exception e2) {
        // }
        // }
        //
        // return null;
    }

    /**
     * @see StatisticsType#POLLS_TOTAL_SUCCESS_FAIL_STATS
     */
    protected JFreeChart renderPollsTotalSuccessFailStats() {
        int successPolls = 0;
        int failPolls = 0;

        Connection dbConn = null;
        try {
            dbConn = db.getConnection();

            Statement surveysSt = dbConn.createStatement();
            ResultSet surveysRs = surveysSt
                    .executeQuery("select id from pollsession");

            while (surveysRs.next()) {
                int surveyId = surveysRs.getInt("id");

                DBManager.threadLocalConnection.set(dbConn);
                Pollsession currentSurvey = db.getPollsessionById(Integer
                        .toString(surveyId));
                DBManager.threadLocalConnection.set(null);

                Statement pollsSt = dbConn.createStatement();
                ResultSet pollsRs = pollsSt
                        .executeQuery("select poll_id, choice_id from results where pollsession_id = "
                                + surveyId);

                while (pollsRs.next()) {
                    if (currentSurvey.getPollById(
                            Integer.toString(pollsRs.getInt("poll_id")))
                            .getCorrectChoice().equals(
                                    Integer.toString(pollsRs
                                            .getInt("choice_id"))))
                        successPolls++;
                    else
                        failPolls++;
                }

                pollsSt.close();
            }
            surveysSt.close();
        }
        catch (SQLException e) {
            e.printStackTrace();
            successPolls = failPolls = -1;
        }
        finally {
            try {
                dbConn.close();
            }
            catch (Exception e2) {
            }
        }

        JFreeChart chart = null;

        if (successPolls != -1) {
            DefaultKeyedValuesDataset dataSet = new DefaultKeyedValuesDataset();

            dataSet.setValue("Correctly answered polls %", successPolls);
            dataSet.setValue("Failed polls %", failPolls);

            PiePlot piePlot = new PiePlot(dataSet);

            chart = new JFreeChart(piePlot);
        }

        return chart;

        // return chart.createBufferedImage((int) dimension.getWidth(),
        // (int) dimension.getHeight());
    }

    /**
     * @see #dimension
     */
    public Dimension getDimension() {
        return dimension;
    }

    /**
     * Fetches common statistics from DB and puts into int array. <br>
     * <ul>
     * <li>0 - poll surveys count</li>
     * <li>1 - polls count</li>
     * <li>2 - choices count</li>
     * <li>3 - users count</li>
     * </ul>
     * 
     * @return Common statistics.
     * @throws SQLException
     *             On DB errors.
     */
    public int[] getCommonStatistics() throws SQLException {
        Connection conn = null;
        int[] stats = null;
        try {
            conn = db.getConnection();

            stats = new int[4];
            String[] tables = { "pollsession", "polls", "choices", "users" };

            for (int i = 0; i < stats.length; i++) {
                Statement st = null;
                try {
                    st = conn.createStatement();
                    ResultSet rs = st.executeQuery("select count(*) from "
                            + tables[i]);

                    if (rs.next())
                        stats[i] = rs.getInt(1);
                    else
                        stats[i] = -1;
                }
                finally {
                    st.close();
                }
            }
        }
        catch (SQLException e) {
            throw e;
        }
        finally {
            try {
                conn.close();
            }
            catch (Exception e2) {
            }
        }

        return stats;
    }

    /**
     * @see StatisticsType#POLLS_WITH_CUSTOM_CHOICE
     */
    protected JFreeChart renderPollsWithCustomChoice() {
        int pollsWithCustomChoices = 0;
        int pollsWithoutCustomChoices = 0;

        Connection conn = null;
        try {
            conn = db.getConnection();
            conn.setAutoCommit(false);

            Statement pollsSt = null;
            ResultSet pollsRs = null;
            try {
                pollsSt = conn.createStatement();
                pollsRs = pollsSt
                        .executeQuery("select customenabled from polls");

                while (pollsRs.next()) {
                    if (pollsRs.getBoolean(1))
                        pollsWithCustomChoices++;
                    else
                        pollsWithoutCustomChoices++;
                }
            }
            finally {
                pollsRs.close();
                pollsSt.close();
            }

            conn.commit();
        }
        catch (Exception e) {
            try {
                pollsWithCustomChoices = pollsWithoutCustomChoices = -1;
                conn.rollback();
            }
            catch (Exception e2) {
            }
        }
        finally {
            try {
                conn.close();
            }
            catch (Exception e) {
            }
        }

        if (pollsWithCustomChoices != -1) {
            final String collumnName = "Comparing polls with and without custom choice";
            DefaultCategoryDataset dataSet = new DefaultCategoryDataset();
            dataSet.addValue((double) pollsWithCustomChoices,
                    "Polls with custom choice", collumnName);
            dataSet.addValue((double) pollsWithoutCustomChoices,
                    "Polls without custom choice", collumnName);

            JFreeChart chart = ChartFactory.createBarChart(collumnName, "",
                    "Quantity", dataSet, PlotOrientation.HORIZONTAL, true,
                    false, false);

            return chart;
        }
        else {
            return null;
        }
    }

    /**
     * @param n
     *            How much users show in top n.
     * 
     * @see StatisticsType#TOP_USERS_POLLS
     */
    public JFreeChart renderTopUsersPolls(int n) {
        Connection dbConn = null;
        List<UserNameAndSuccessAndFailCount> usersStats = new ArrayList<UserNameAndSuccessAndFailCount>();

        try {
            dbConn = db.getConnection();

            Statement surveysSt = dbConn.createStatement();
            ResultSet surveysRs = surveysSt
                    .executeQuery("select id from pollsession");

            while (surveysRs.next()) {
                int surveyId = surveysRs.getInt("id");

                DBManager.threadLocalConnection.set(dbConn);
                Pollsession currentSurvey = db.getPollsessionById(Integer
                        .toString(surveyId));
                DBManager.threadLocalConnection.set(null);

                Statement pollsSt = dbConn.createStatement();
                ResultSet pollsRs = pollsSt
                        .executeQuery("select poll_id, choice_id, user_name from results where pollsession_id = "
                                + surveyId);

                while (pollsRs.next()) {
                    UserNameAndSuccessAndFailCount statsForCurrentUser = findInListOrAdd(
                            usersStats, pollsRs.getString("user_name"));

                    if (currentSurvey.getPollById(
                            Integer.toString(pollsRs.getInt("poll_id")))
                            .getCorrectChoice().equals(
                                    Integer.toString(pollsRs
                                            .getInt("choice_id"))))
                        statsForCurrentUser.successCount++;
                    else
                        statsForCurrentUser.failCount++;
                }

                pollsSt.close();
            }
            surveysSt.close();
        }
        catch (SQLException e) {
            e.printStackTrace();
            usersStats = null;
        }
        finally {
            try {
                dbConn.close();
            }
            catch (Exception e2) {
            }
        }

        JFreeChart chart = null;

        if (usersStats != null) {
            final String collumnName = "Users";
            DefaultCategoryDataset dataSet = new DefaultCategoryDataset();

            for (UserNameAndSuccessAndFailCount userStats : usersStats) {
                dataSet.addValue((double) userStats.successCount, userStats
                        .getUserName(), collumnName);
            }

            // TODO: This adds users with zero passed polls, but they aren't
            // shown on chart.
            try {
                dbConn = db.getConnection();

                Statement usersSt = dbConn.createStatement();
                ResultSet usersRs = usersSt
                        .executeQuery("select userName from users");

                while (usersRs.next()) {
                    String userName = usersRs.getString(1);

                    findInListOrAdd(usersStats, userName);
                }

                usersRs.close();
                usersSt.close();
            }
            catch (SQLException e) {
                e.printStackTrace();
                usersStats = null;
            }
            finally {
                try {
                    dbConn.close();
                }
                catch (Exception e2) {
                }
            }

            chart = ChartFactory.createBarChart(collumnName, "", "Quantity",
                    dataSet, PlotOrientation.HORIZONTAL, true, false, false);

            return chart;
        }

        return chart;
    }

    /**
     * Used to store user and success/fail count pairs.
     */
    public class UserNameAndSuccessAndFailCount {

        protected String userName = null;

        public int successCount = 0;

        public int failCount = 0;

        public UserNameAndSuccessAndFailCount(String userName) {
            if (userName == null)
                throw new IllegalArgumentException(
                        "User ID should be greater then zero.");

            this.userName = userName;
        }

        public String getUserName() {
            return userName;
        }

    }

    public UserNameAndSuccessAndFailCount findInListOrAdd(
            List<UserNameAndSuccessAndFailCount> list, String userName) {
        for (UserNameAndSuccessAndFailCount user : list)
            if (user.getUserName().equals(userName))
                return user;

        UserNameAndSuccessAndFailCount newUser = new UserNameAndSuccessAndFailCount(
                userName);
        list.add(newUser);

        return newUser;
    }

}
