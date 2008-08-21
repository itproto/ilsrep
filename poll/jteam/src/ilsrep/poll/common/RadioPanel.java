package ilsrep.poll.common;
import ilsrep.poll.client.PollClientGUI;
import java.*;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JLabel;
import javax.swing.JRadioButton;
import javax.swing.*;
import java.awt.*;
import java.awt.event.*;
import java.util.Enumeration;
public class RadioPanel extends JPanel implements ActionListener {
WaitThread waiter=null;
public String reply="-1";
public class WaitThread extends Thread {
public String reply="-1";
public void run() {
Boolean a=true;
while(reply.equals("-1")){
try{sleep(250);} catch(Exception arg) {System.out.println(arg.getMessage());}
}
System.out.println("lol");
}
}

private Enumeration e;
RadioPanel(ButtonGroup group){
  super(new BorderLayout());
 JPanel radioPanel = new JPanel(new GridLayout(0, 1));
 e=group.getElements();

    while ( e.hasMoreElements() ) {
         radioPanel.add((JRadioButton)e.nextElement());
     }
 e=group.getElements();
add(radioPanel, BorderLayout.LINE_START);
JButton b2 = new JButton("OK");
add(b2, BorderLayout.CENTER);
b2.setActionCommand("Go");
b2.addActionListener(this);
setBorder(BorderFactory.createEmptyBorder(20,20,20,20));

}

public void actionPerformed(ActionEvent act) {
if(act.getActionCommand().equals("Go")) {
 while ( e.hasMoreElements() ) {
         JRadioButton jrb =(JRadioButton)e.nextElement();
if (jrb.isSelected()) {reply=jrb.getActionCommand();
System.out.println(jrb.getActionCommand());

}
     }

}   
} 
public void startme(){
waiter=new WaitThread();
waiter.start();
}
}


