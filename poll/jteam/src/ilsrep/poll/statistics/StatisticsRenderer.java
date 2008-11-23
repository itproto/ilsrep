package ilsrep.poll.statistics;

import ilsrep.poll.common.model.Poll;
import ilsrep.poll.common.model.Pollsession;
import ilsrep.poll.server.db.DBManager;

import java.awt.Dimension;
import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.sql.Timestamp;

import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PiePlot;
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
        }

        return renderedChart;
    }

    /**
     * @see StatisticsType#SURVEYS_TOTAL_SUCCESS_FAIL_STATS
     */
    protected JFreeChart renderSurveysTotalSuccessFailStats() {
        int successSurveys = 0;
        int failSurveys = 0;

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

                Statement surveySeancesSt = dbConn.createStatement();
                ResultSet surveySeancesRs = surveySeancesSt
                        .executeQuery("select distinct date from results where pollsession_id = "
                                + surveyId);

                while (surveySeancesRs.next()) {
                    Timestamp currentSeanceDate = surveySeancesRs
                            .getTimestamp(1);

                    System.out.println(currentSeanceDate.toString());

                    int successPolls = 0;

                    for (Poll poll : currentSurvey.getPolls()) {
                        Statement pollResultSt = dbConn.createStatement();
                        ResultSet pollResultRs = pollResultSt
                                .executeQuery("select choice_id from results where poll_id = "
                                        + poll.getId()
                                        + " and\"date = "
                                        + currentSeanceDate.toString() + "\"");

                        if (pollResultRs.next()
                                && Integer.toString(
                                        pollResultRs.getInt("choice_id"))
                                        .equals(poll.getCorrectChoice()))
                            successPolls++;

                        pollResultSt.close();
                    }

                    if (((double) successPolls)
                            / currentSurvey.getPolls().size() > Integer
                            .parseInt(currentSurvey.getMinScore()))
                        successSurveys++;
                    else
                        failSurveys++;
                }
            }

            System.out.println(successSurveys + " " + failSurveys);
        }
        catch (SQLException e) {
            e.printStackTrace();
            successSurveys = failSurveys = -1;
        }
        finally {
            try {
                dbConn.close();
            }
            catch (Exception e2) {
            }
        }

        return null;
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
            }
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
}
