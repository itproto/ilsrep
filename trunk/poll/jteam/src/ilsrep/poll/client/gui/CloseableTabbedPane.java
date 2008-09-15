package ilsrep.poll.client.gui;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

import javax.swing.AbstractButton;
import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.Icon;
import javax.swing.JButton;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTabbedPane;
import javax.swing.plaf.basic.BasicButtonUI;

/**
 * "Tabbed pane" that differs from <code>JTabbedPane</code> that you can close
 * tabs.
 * 
 * @author TKOST
 * 
 */
public class CloseableTabbedPane extends JTabbedPane {

    /**
     * Serial version UID.
     */
    private static final long serialVersionUID = -8027584062668628199L;

    /**
     * Adds tab with close button, without icon.
     * 
     * @param title
     *            Tab title.
     * @param content
     *            Tab content.
     */
    public void addCloseableTab(String title, JPanel content) {
        addCloseableTab(title, content, null);
    }

    /**
     * Adds tab with close button and with icon.
     * 
     * @param title
     *            Tab title.
     * @param content
     *            Tab content.
     * @param tabIcon
     *            Tab icon.
     */
    public void addCloseableTab(String title, JPanel content, Icon tabIcon) {
        if (title == null || content == null)
            throw new NullPointerException(
                    "Title and content should not be null!");

        addTab(title, content);

        JPanel titlePanel = new JPanel();

        BoxLayout titlePanelLayout = new BoxLayout(titlePanel,
                BoxLayout.LINE_AXIS);

        titlePanel.setLayout(titlePanelLayout);

        // Space in pixels between icon(if present), text title and close
        // button.
        final int titleElementsSpace = 6;

        if (tabIcon != null) {
            titlePanel.add(new JLabel(tabIcon));

            titlePanel.add(Box.createRigidArea(new Dimension(
                    titleElementsSpace, titleElementsSpace)));

        }

        titlePanel.add(new JLabel(title));

        titlePanel.add(Box.createRigidArea(new Dimension(titleElementsSpace,
                titleElementsSpace)));

        titlePanel.add(new TabCloseButton(titlePanel));

        setTabComponentAt(getTabCount() - 1, titlePanel);
    }

    /**
     * Button for closing tab.<br>
     * Idea and almost all code copied from Java tutorial.
     * 
     * @author Modified code fromm Java tutorial.
     * 
     */
    private class TabCloseButton extends JButton implements ActionListener {

        /**
         * Serial version UID.
         */
        private static final long serialVersionUID = 6860163366288470196L;

        /**
         * Panel that owns this button.
         */
        private JPanel owningPanel = null;

        /**
         * Creates new button for closing tab.
         */
        public TabCloseButton(JPanel owningPanel) {
            this.owningPanel = owningPanel;

            {
                final int size = 17;

                Dimension sizeD = new Dimension(size, size);

                TabCloseButton.this.setPreferredSize(sizeD);
                TabCloseButton.this.setMaximumSize(sizeD);
                TabCloseButton.this.setMinimumSize(sizeD);
            }

            setToolTipText("Close tab");

            setUI(new BasicButtonUI());

            setContentAreaFilled(false);

            setBorder(BorderFactory.createEtchedBorder());

            setBorderPainted(false);

            addMouseListener(new MouseAdapter() {

                public void mouseEntered(MouseEvent e) {
                    Component component = e.getComponent();
                    if (component instanceof AbstractButton) {
                        AbstractButton button = (AbstractButton) component;
                        button.setBorderPainted(true);
                    }
                }

                public void mouseExited(MouseEvent e) {
                    Component component = e.getComponent();
                    if (component instanceof AbstractButton) {
                        AbstractButton button = (AbstractButton) component;
                        button.setBorderPainted(false);
                    }
                }
            });

            setRolloverEnabled(true);

            addActionListener(this);
        }

        /**
         * @see java.awt.event.ActionListener#actionPerformed(java.awt.event.ActionEvent
         *      )
         */
        @Override
        public void actionPerformed(ActionEvent e) {
            int indexToRemove = -1;
            if ((indexToRemove = indexOfTabComponent(owningPanel)) != -1)
                CloseableTabbedPane.this.remove(indexToRemove);
        }

        /**
         * Does nothing for this button.
         * 
         * @see javax.swing.JButton#updateUI()
         */
        @Override
        public void updateUI() {
        }

        /**
         * Paints this button.
         * 
         * @see javax.swing.JComponent#paintComponent(java.awt.Graphics)
         */
        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);

            Graphics2D g2 = (Graphics2D) g.create();

            if (getModel().isPressed()) {
                g2.translate(1, 1);
            }

            g2.setStroke(new BasicStroke(2));

            g2.setColor(Color.BLACK);

            if (getModel().isRollover()) {
                g2.setColor(Color.RED);
            }

            int delta = 6;

            g2.drawLine(delta, delta, getWidth() - delta - 1, getHeight()
                    - delta - 1);

            g2.drawLine(getWidth() - delta - 1, delta, delta, getHeight()
                    - delta - 1);

            g2.dispose();
        }
    }

}
