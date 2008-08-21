package ilsrep.poll.common;
import ilsrep.poll.common.RadioPanel;
import java.util.List;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JLabel;
import javax.swing.JRadioButton;
import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import java.util.Enumeration;
public class MainWindow extends JFrame  {
public MainWindow(){
JLabel jlbHelloWorld = new JLabel("Poll");

		this.setSize(1000, 1000);
		 
		//this.setVisible(true);

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

public String getChoice(ButtonGroup group, String query){
this.setTitle(query);
 RadioPanel newContentPane = new RadioPanel(group);
        newContentPane.setOpaque(true); //content panes must be opaque
        setContentPane(newContentPane);

        //Display the window.
        pack();
        setVisible(true);
while (newContentPane.reply.equals("-1")){
try{Thread.sleep(250);} catch(Exception arg) {System.out.println(arg.getMessage());}
}

return newContentPane.reply;
	
}

}