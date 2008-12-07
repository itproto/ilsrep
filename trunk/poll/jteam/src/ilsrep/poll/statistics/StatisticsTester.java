/**
 * 
 */
package ilsrep.poll.statistics;

import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.server.db.SQLiteDBManager;

import java.awt.Dimension;

import org.jfree.chart.ChartFrame;
import org.jfree.chart.JFreeChart;

/**
 * @author Taras Kostiak
 * 
 */
public class StatisticsTester {

    public static void main(String[] args) throws Exception {
        DBManager db = new SQLiteDBManager(null,
                SQLiteDBManager.DEFAULT_DB_FILE);

        StatisticsRenderer renderer = new StatisticsRenderer(db, new Dimension(
                640, 480));

        JFreeChart chart = renderer
                .renderStatisticsChart(StatisticsType.POLLS_WITH_CUSTOM_CHOICE);

        ChartFrame frm = new ChartFrame("test", chart);
        frm.setVisible(true);
    }

}
