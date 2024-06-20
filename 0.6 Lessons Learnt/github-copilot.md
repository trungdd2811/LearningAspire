# Lessons learned when using Github Copilot

## 1. Github Copilot can suggests wrong code based on the current context

When I was writing the code for the Aspire App Host, I found that Github Copilot suggested the wrong code based on the current context. For example:

* Current context: already use a function to add SQL Server
* Question: How to add a new SQL Server to the Aspire app host?

![add-sqlserver-container-ask.PNG](0.6%20Lessons%20Learnt/Images/add-sqlserver-container-ask.PNG)

![add-sqlserver-container-result.PNG](0.6%20Lessons%20Learnt/Images/add-sqlserver-container-result.PNG)

## 2. Github Copilot suggests wrong code due to missing information on the platform. For example:

Aspire is a new technology that is not well-known. And Github Copilot does not have enough information about it. So, it suggests the wrong code. For example:

![add-mongo-container-ask.PNG](0.6%20Lessons%20Learnt/Images/add-mongo-container-ask.PNG)

![add-mongo-container-result.PNG](0.6%20Lessons%20Learnt/Images/add-mongo-container-result.PNG)

![add-sqlserver-container-wrong-result.png](0.6%20Lessons%20Learnt/Images/add-sqlserver-container-wrong-result.png)
