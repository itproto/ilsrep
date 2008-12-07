package ilsrep.poll.statistics;

/**
 * Type of statistics chart.
 * 
 * @author TKOST
 * 
 */
public enum StatisticsType {

    /**
     * Pie chart with percent of succeed and failed surveys.
     */
    SURVEYS_TOTAL_SUCCESS_FAIL_STATS,

    /**
     * Pie chart with percent of succeed and failed polls.
     */
    POLLS_TOTAL_SUCCESS_FAIL_STATS,

    /**
     * Bar chart that shows percent of polls with custom choices and w/o ones.
     */
    POLLS_WITH_CUSTOM_CHOICE;

    /**
     * @see java.lang.Enum#toString()
     */
    @Override
    public String toString() {
        String typeString = null;

        switch (this) {
            case SURVEYS_TOTAL_SUCCESS_FAIL_STATS:
                typeString = "0";
                break;

            case POLLS_TOTAL_SUCCESS_FAIL_STATS:
                typeString = "1";
                break;

            case POLLS_WITH_CUSTOM_CHOICE:
                typeString = "2";
                break;
        }

        return typeString;
    }

}
