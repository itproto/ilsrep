package ilsrep.poll.client;

import java.util.LinkedList;

/**
 * Saves name and choice pairs.
 * 
 * @author DCR
 */
public class AnswerSaver {
public float minScore,i,n;
public String testMode;
    /**
     * This class represents name and choice selection pair.
     */
    protected class AnswerHelper {

        /**
         * Name
         */
        String name = null;

        /**
         * Choice selection.
         */
        String selection = null;
         String correct = "FAIL";
    };

    /**
     * This list stores name and choice selection pairs.
     */
    protected LinkedList<AnswerHelper> answerList = null;

    /**
     * Default constructor.
     */
    public AnswerSaver() {
        answerList = new LinkedList<AnswerHelper>();
    }

    /**
     * Adds name and choice selection pair to LogSaver.
     * 
     * @param desc
     *            Name.
     * @param sid
     *            Choice selection.
     */
    public void pushAnswer(String poll, String answer, String correct) {
        AnswerHelper listElement = new AnswerHelper();
        listElement.name = poll;
        listElement.selection = answer;
        listElement.correct = correct;
        answerList.add(listElement);

    }

    /**
     * Prints all saved name and choice selection pairs.<br>
     * 
     * TODO: (BUG) This removes list from memory, which could be used later.
     * DRC: you propose i should just read the list instead of clearing it?
     */
    public void popAnswer() {
        while (!(answerList.size() == 0)) {
            AnswerHelper cur = answerList.removeFirst();
            
           if(this.testMode.compareTo("true")==0) System.out.println(cur.name + " => " + cur.selection + "=>"+cur.correct+"\n");
           else System.out.println(cur.name + " => " + cur.selection + "\n");
           
            if (cur.correct=="PASS") this.i++;
            this.n++;
        }

       
      if(this.testMode.compareTo("true")==0){
	System.out.println("Your score "+Float.toString(this.i/this.n));
	
	if ((this.i/this.n)>=this.minScore){ System.out.println("You pass");
	} else{
		 System.out.println("You fail");
	 }
	 }
	};
}
