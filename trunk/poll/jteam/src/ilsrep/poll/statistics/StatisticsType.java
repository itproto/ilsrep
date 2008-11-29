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
    POLLS_TOTAL_SUCCESS_FAIL_STATS;

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
        }

        return typeString;
    }

    // /**
    // * Returns int code of statistics type.
    // *
    // * @return Statistics type code.
    // */
    // public int getTypeId() {
    // int type = -1;
    //
    // switch (this) {
    // case SURVEYS_TOTAL_SUCCESS_FAIL_STATS:
    // type = 0;
    // break;
    //
    // case POLLS_TOTAL_SUCCESS_FAIL_STATS:
    // type = 1;
    // break;
    // }
    //
    // return type;
    // }
    //
    // public static StatisticsType getEnumByTypeId(int type) {
    // StatisticsType typeEnum = null;
    //
    // switch (type) {
    // case 0:
    // typeEnum = SURVEYS_TOTAL_SUCCESS_FAIL_STATS;
    // break;
    // case 1:
    // typeEnum = POLLS_TOTAL_SUCCESS_FAIL_STATS;
    // break;
    // }
    //
    // return typeEnum;
    // }

}
