package ilsrep.poll.web;

import ilsrep.poll.server.db.DBManager;
import ilsrep.poll.server.db.SQLiteDBManager;
import ilsrep.poll.statistics.StatisticsRenderer;
import ilsrep.poll.statistics.StatisticsType;

import java.awt.Dimension;
import java.io.IOException;
import java.io.OutputStream;
import java.io.PrintStream;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jfree.chart.ChartUtilities;
import org.jfree.chart.JFreeChart;

/**
 * @author TKOST
 * 
 */
public class StatisticsServlet extends HttpServlet {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = 3153611000417201660L;

    /**
     * DB, to fetch statistics from.
     */
    protected DBManager db = null;

    /**
     * Statistics chart renderer.
     */
    protected StatisticsRenderer renderer = null;

    /**
     * Default dimension of statistics image.
     */
    public static final Dimension DEFAULT_STATS_IMAGE_DIMENSION = new Dimension(
            640, 480);

    /**
     * Processes a GET request.
     * 
     * @param request
     *            the request.
     * @param response
     *            the response.
     * 
     * @throws ServletException
     *             if there is a servlet related problem.
     * @throws IOException
     *             if there is an I/O problem.
     */
    public void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        OutputStream out = null;
        try {
            if (db == null) {
                db = new SQLiteDBManager(null, getServletContext().getRealPath(
                        "/")
                        + "/pollserver.db");
            }

            if (renderer == null) {
                renderer = new StatisticsRenderer(db, new Dimension(640, 480));
            }

            out = response.getOutputStream();

            JFreeChart chart = renderer
                    .renderStatisticsChart(StatisticsType.POLLS_TOTAL_SUCCESS_FAIL_STATS);

            if (chart != null) {
                response.setContentType("image/png");
                ChartUtilities.writeChartAsPNG(out, chart,
                        (int) DEFAULT_STATS_IMAGE_DIMENSION.getWidth(),
                        (int) DEFAULT_STATS_IMAGE_DIMENSION.getHeight());
            }
            else {
                (new PrintStream(out)).print("ERROR");
            }
        }
        catch (Exception e) {
            System.err.println(e.toString());
        }
        finally {
            out.close();
        }
    }
}
