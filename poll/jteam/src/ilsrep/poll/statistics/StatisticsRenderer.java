package ilsrep.poll.statistics;

import java.awt.Dimension;
import java.sql.SQLException;
import java.util.List;

import org.jfree.chart.ChartFactory;
import org.jfree.chart.JFreeChart;
import org.jfree.chart.plot.PiePlot;
import org.jfree.chart.plot.PlotOrientation;
import org.jfree.data.category.DefaultCategoryDataset;
import org.jfree.data.general.DefaultKeyedValuesDataset;

import webservice.endpoint.Exception_Exception;
import webservice.endpoint.WebJPoll_Service;

/**
 * @author TKOST
 */
public class StatisticsRenderer {

    /**
     * Database to read statistics from.
     */
    protected WebJPoll_Service service = null;

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
    public StatisticsRenderer(WebJPoll_Service service, Dimension dimension) {
        this.service = service;
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
        try {
            List<Double> statisticsData = service
                    .getWebJPollPort()
                    .fetchStatistics(
                            webservice.endpoint.StatisticsType.POLLS_TOTAL_SUCCESS_FAIL_STATS);

            if (statisticsData != null) {
                int successPolls = (int) statisticsData.get(0).doubleValue();
                int failPolls = (int) statisticsData.get(1).doubleValue();

                JFreeChart chart = null;

                if (successPolls != -1) {
                    DefaultKeyedValuesDataset dataSet = new DefaultKeyedValuesDataset();

                    dataSet
                            .setValue("Correctly answered polls %",
                                    successPolls);
                    dataSet.setValue("Failed polls %", failPolls);

                    PiePlot piePlot = new PiePlot(dataSet);

                    chart = new JFreeChart(piePlot);
                }
                return chart;
            }
            else {
                return null;
            }
        }
        catch (Exception_Exception e) {
            return null;
        }
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
        try {
            List<Integer> stats = service.getWebJPollPort()
                    .getCommonStatistics();

            int[] statsArray = new int[stats.size()];

            for (int i = 0; i < stats.size(); i++) {
                statsArray[i] = stats.get(i);
            }

            return statsArray;
        }
        catch (Exception_Exception e) {
            return null;
        }

    }

    /**
     * @see StatisticsType#POLLS_WITH_CUSTOM_CHOICE
     */
    protected JFreeChart renderPollsWithCustomChoice() {
        int pollsWithCustomChoices = 0;
        int pollsWithoutCustomChoices = 0;

        List<Double> statisticsData = null;

        try {
            statisticsData = service
                    .getWebJPollPort()
                    .fetchStatistics(
                            webservice.endpoint.StatisticsType.POLLS_WITH_CUSTOM_CHOICE);
        }
        catch (Exception_Exception e) {
            pollsWithCustomChoices = pollsWithoutCustomChoices = -1;
        }

        pollsWithCustomChoices = (int) statisticsData.get(0).doubleValue();
        pollsWithoutCustomChoices = (int) statisticsData.get(1).doubleValue();

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
        List<Results> usersStats = null;
        try {
            usersStats = service.getWebJPollPort().fetchTopUsersPolls(n);
        }
        catch (Exception_Exception e) {
        }

        JFreeChart chart = null;

        if (usersStats != null) {
            final String collumnName = "Users";
            DefaultCategoryDataset dataSet = new DefaultCategoryDataset();

            for (Results userStats : usersStats) {
                dataSet.addValue((double) Double.parseDouble(userStats
                        .getPercent()) * 100, userStats.getName(), collumnName);
            }

            chart = ChartFactory.createBarChart(collumnName, "", "Percent",
                    dataSet, PlotOrientation.HORIZONTAL, true, false, false);

            return chart;
        }

        return chart;
    }

}
