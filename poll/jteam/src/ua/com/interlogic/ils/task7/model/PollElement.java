package ua.com.interlogic.ils.task7.model;

import java.util.List;
import java.io.*;
import javax.xml.bind.annotation.XmlAttribute;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlElementWrapper;
import javax.xml.bind.annotation.XmlRootElement;

/**
 * The "poll" element.
 * 
 * @author Taras Kostiak
 * edited by drax
 */
@XmlRootElement(name = "poll")
public class PollElement {

    protected String id = null;

    protected String name = null;

    protected DescriptionElement description = null;

    protected List<ChoiceElement> choices = null;
    
  
   public String Selection;
 
   
public void setsid(){
	int sid;
	try {InputStreamReader converter = new InputStreamReader(System.in);
         BufferedReader in = new BufferedReader(converter);
	sid = Integer.parseInt(in.readLine());;
	
	 for (ChoiceElement el : this.getChoices()) {
      if (sid==Integer.parseInt(el.getId())) this.Selection=el.getName();
                       
                        
	}
	} catch(Exception ex){};
	}
    /**
     * @see #description
     */
      
     
    public DescriptionElement getDescription() {
        return description;
    }

    /**
     * @see #description
     */
    public void setDescription(DescriptionElement description) {
        this.description = description;
    }

    /**
     * @see #choices
     */
    public List<ChoiceElement> getChoices() {
        return choices;
    }

    /**
     * @see #choices
     */
    @XmlElementWrapper(name = "choices")
    @XmlElementRef
    public void setChoices(List<ChoiceElement> choices) {
        this.choices = choices;
    }

    /**
     * @see #id
     */
    @XmlAttribute
    public String getId() {
        return id;
    }

    /**
     * @see #id
     */
    public void setId(String id) {
        this.id = id;
    }

    /**
     * @see #name
     */
    @XmlAttribute
    public String getName() {
        return name;
    }

    /**
     * @see #name
     */
    public void setName(String name) {
        this.name = name;
    }
// started editing

public void QueryUser(){
	try{ for (int i=0; i<40; i++) System.out.println();;} catch ( Exception ex){};
	System.out.println("Name: " + this.getName());
            System.out.println("Desription: " + this.getDescription().getValue());
            for (ChoiceElement el : this.getChoices()) {
                System.out.println("| " + el.getId() + "| "
                        + el.getName());
                       
                        
	}
	 this.setsid();
	 try{
for (int i=0; i<40; i++) System.out.println();;} catch ( Exception ex){};
}
}