package ilsrep.poll.web;

import ilsrep.poll.statistics.StatisticsRenderer;
import ilsrep.poll.statistics.StatisticsType;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.io.OutputStream;
import java.net.MalformedURLException;
import java.net.URL;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.xml.namespace.QName;

import org.jfree.chart.ChartUtilities;
import org.jfree.chart.JFreeChart;

import webservice.endpoint.WebJPoll_Service;

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
     * Default dimension of statistics image.
     */
    public static final Dimension DEFAULT_STATS_IMAGE_DIMENSION = new Dimension(
            640, 480);

    /**
     * Name of "type" parameter.
     */
    public static final String PARAMETER_TYPE_NAME = "type";

    /**
     * Name of "count" parameter.
     */
    public static final String PARAMETER_TYPE_COUNT = "count";

    /**
     * Statistics chart renderer.
     */
    protected StatisticsRenderer renderer = null;

    /**
     * Creates <code>StatisticsRenderer</code> with default dimension and using
     * given <code>ServletContext</code> to get db file.
     * 
     * @param context
     *            <code>ServletContext</code>, to get db file.
     * @return
     */
    public static StatisticsRenderer createDefaultRenderer(String name,
            String port) {
        try {
            StatisticsRenderer renderer = null;

            WebJPoll_Service service = null;
            QName serviceName = new QName("http://endpoint.webservice/",
                    "WebJPoll");
            URL url = new URL("http://" + name + ":" + port
                    + "/WebJPoll/WebJPoll?wsdl");
            service = new WebJPoll_Service(url, serviceName);

            renderer = new StatisticsRenderer(service,
                    DEFAULT_STATS_IMAGE_DIMENSION);
            return renderer;
        }
        catch (MalformedURLException e) {
            return null;
        }

    }

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
        if (renderer == null)
            renderer = createDefaultRenderer(request.getServerName(), Integer
                    .toString(request.getServerPort()));

        OutputStream out = null;
        try {
            String type = request.getParameter(PARAMETER_TYPE_NAME);

            out = response.getOutputStream();

            if (type == null) {
                ChartUtilities.writeBufferedImageAsPNG(out,
                        renderErrorImage("Type not specified!"));
            }
            else {
                JFreeChart chart = null;

                if (type.equals(StatisticsType.SURVEYS_TOTAL_SUCCESS_FAIL_STATS
                        .toString())) {
                    chart = renderer
                            .renderStatisticsChart(StatisticsType.SURVEYS_TOTAL_SUCCESS_FAIL_STATS);
                }
                else
                    if (type
                            .equals(StatisticsType.POLLS_TOTAL_SUCCESS_FAIL_STATS
                                    .toString())) {
                        chart = renderer
                                .renderStatisticsChart(StatisticsType.POLLS_TOTAL_SUCCESS_FAIL_STATS);
                    }
                    else
                        if (type.equals(StatisticsType.POLLS_WITH_CUSTOM_CHOICE
                                .toString())) {
                            chart = renderer
                                    .renderStatisticsChart(StatisticsType.POLLS_WITH_CUSTOM_CHOICE);
                        }
                        else
                            if (type.equals(StatisticsType.TOP_USERS_POLLS
                                    .toString())) {
                                String count = request
                                        .getParameter(PARAMETER_TYPE_COUNT);
                                try {
                                    chart = renderer
                                            .renderTopUsersPolls(Integer
                                                    .parseInt(count));
                                }
                                catch (NumberFormatException e) {
                                    final int DEFAULT_COUNT = 5;

                                    chart = renderer
                                            .renderTopUsersPolls(DEFAULT_COUNT);
                                }
                            }
                            else
                                ChartUtilities.writeBufferedImageAsPNG(out,
                                        renderErrorImage("No such type!"));

                if (chart != null) {
                    response.setContentType("image/png");
                    ChartUtilities.writeChartAsPNG(out, chart, (int) renderer
                            .getDimension().getWidth(), (int) renderer
                            .getDimension().getHeight());
                }
                else {
                    ChartUtilities.writeBufferedImageAsPNG(out,
                            renderErrorImage("Chart rendering error!"));
                }
            }
        }
        catch (Exception e) {
            System.err.println(e.toString());
        }
        finally {
            out.close();
        }
    }

    /**
     * Generates error image.
     * 
     * @param errorString
     *            Error message.
     * 
     * @return Generated error image.
     */
    public BufferedImage renderErrorImage(String errorString) {
        // Image width and height.
        float width = (float) renderer.getDimension().getWidth();
        float height = (float) renderer.getDimension().getHeight();

        BufferedImage errorImage = new BufferedImage((int) width, (int) height,
                BufferedImage.TYPE_4BYTE_ABGR);

        Graphics2D g = errorImage.createGraphics();

        g.setColor(new Color(255, 0, 0));

        g.drawString("ERROR WHILE RENDERING STATISTICS:", width / 4, height
                * (float) 0.45);

        g
                .drawString(errorString, width / 4 + width / 10, height
                        * (float) 0.55);

        return errorImage;
    }

}
