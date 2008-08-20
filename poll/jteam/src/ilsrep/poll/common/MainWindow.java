package ilsrep.poll.common;
import java.util.List;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JLabel;
public class MainWindow extends JFrame{
public MainWindow(){
JLabel jlbHelloWorld = new JLabel("Poll");
		this.add(jlbHelloWorld);
		this.setSize(300, 300);
		// pack();
		this.setVisible(true);
}
public String askUser(String query){
 String s = JOptionPane.showInputDialog(query);
return s;
}
public int askUserChoice(String query,Object[] options ){
 int s = JOptionPane.showOptionDialog(this,
    query,
    "Please choose",
    JOptionPane.YES_NO_CANCEL_OPTION,
    JOptionPane.QUESTION_MESSAGE,
    null,
    options,
    options[0]);
return s;
}
public void alert(String alertion){
 JOptionPane.showMessageDialog(null, alertion, alertion, JOptionPane.ERROR_MESSAGE);


}
}