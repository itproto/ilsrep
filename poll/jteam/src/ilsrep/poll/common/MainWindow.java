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


		
	 this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);	 
		//this.setVisible(true);

}
public String askUser(String query){
 //String s = JOptionPane.showInputDialog(query);
  JOptionPane pane = new JOptionPane();
pane.setMessageType(JOptionPane.QUESTION_MESSAGE);
pane.setMessage(query);
Object[] a= {"OK"};
pane.setOptions(a);
pane.setOptionType(JOptionPane.DEFAULT_OPTION);
pane.setWantsInput(true);
JDialog dialog = pane.createDialog(this, query);
dialog.show();
String selectedValue = (String)pane.getInputValue();


return selectedValue;
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

public Boolean askYesNo(String query){
Object[] options = {"Yes",
                    "No"};
 int s = JOptionPane.showOptionDialog(this,
    query,
    "Please choose",
    JOptionPane.YES_NO_CANCEL_OPTION,
    JOptionPane.QUESTION_MESSAGE,
    null,
    options,
    options[0]);
return (s==0)?true : false;


}

public String getChoice(ButtonGroup group, String query){
this.setTitle("Please choose");
 RadioPanel newContentPane = new RadioPanel(group,query);
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