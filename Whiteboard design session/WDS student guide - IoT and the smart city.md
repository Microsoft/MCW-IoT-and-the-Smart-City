![Microsoft Cloud Workshops](https://github.com/Microsoft/MCW-Template-Cloud-Workshop/raw/main/Media/ms-cloud-workshop.png 'Microsoft Cloud Workshops')

<div class="MCWHeader1">
IoT and the Smart City
</div>

<div class="MCWHeader2">
Whiteboard design session student guide
</div>

<div class="MCWHeader3">
June 2020
</div>

Information in this document, including URL and other Internet Web site references, is subject to change without notice. Unless otherwise noted, the example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious, and no association with any real company, organization, product, domain name, e-mail address, logo, person, place or event is intended or should be inferred. Complying with all applicable copyright laws is the responsibility of the user. Without limiting the rights under copyright, no part of this document may be reproduced, stored in or introduced into a retrieval system, or transmitted in any form or by any means (electronic, mechanical, photocopying, recording, or otherwise), or for any purpose, without the express written permission of Microsoft Corporation.

Microsoft may have patents, patent applications, trademarks, copyrights, or other intellectual property rights covering subject matter in this document. Except as expressly provided in any written license agreement from Microsoft, the furnishing of this document does not give you any license to these patents, trademarks, copyrights, or other intellectual property.

The names of manufacturers, products, or URLs are provided for informational purposes only, and Microsoft makes no representations and warranties, either expressed, implied, or statutory, regarding these manufacturers or the use of the products with any Microsoft technologies. The inclusion of a manufacturer or product does not imply endorsement of Microsoft of the manufacturer or product. Links may be provided to third-party sites. Such sites are not under the control of Microsoft and Microsoft is not responsible for the contents of any linked site or any link contained in a linked site, or any changes or updates to such sites. Microsoft is not responsible for webcasting or any other form of transmission received from any linked site. Microsoft is providing these links to you only as a convenience, and the inclusion of any link does not imply endorsement of Microsoft of the site or the products contained therein.

© 2020 Microsoft Corporation. All rights reserved.

Microsoft and the trademarks listed at <https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/Usage/General.aspx> are trademarks of the Microsoft group of companies. All other trademarks are the property of their respective owners.

**Contents**

<!-- TOC -->

- [IoT and the Smart City whiteboard design session student guide](#iot-and-the-smart-city-whiteboard-design-session-student-guide)
  - [Abstract and learning objectives](#abstract-and-learning-objectives)
  - [Step 1: Review the customer case study](#step-1-review-the-customer-case-study)
      - [Customer situation](#customer-situation)
    - [Customer needs](#customer-needs)
    - [Customer objections](#customer-objections)
    - [Infographic for common scenarios](#infographic-for-common-scenarios)
  - [Step 2: Design a proof of concept solution](#step-2-design-a-proof-of-concept-solution)
  - [Step 3: Present the solution](#step-3-present-the-solution)
  - [Wrap-up](#wrap-up)
  - [Additional references](#additional-references)

<!-- /TOC -->

# IoT and the Smart City whiteboard design session student guide

## Abstract and learning objectives

This whiteboard design session is designed to help you gain a better understanding of implementing architectures that use IoT data in new and innovative ways. You will design an IoT workflow that begins with a local IoT edge device that collects and analyzes data from various sensors that are connected to it, and intelligently aggregates and sends that data to the cloud when anomalies are detected. Once the data is uploaded, it is sent to a time-series database for rapid analysis alongside other classes of IoT data to spot and act on correlated information in real-time. You will also configure alerts when certain thresholds are exceeded and configure a remote monitoring accelerator solution that manages and sends control messages to IoT devices located within the city limits.

At the end of this whiteboard design session, you will be better able to design an end-to-end IoT solution that processes and analyzes data both in the field and in the cloud.

## Step 1: Review the customer case study

**Outcome**

Analyze your customer's needs.

Timeframe: 15 minutes

Directions: With all participants in the session, the facilitator/SME presents an overview of the customer case study along with technical tips.

1.  Meet your table participants and trainer.

2.  Read all of the directions for steps 1-3 in the student guide.

3.  As a table team, review the following customer case study.

#### Customer situation

Fabrikam City council has conducted a six-month study of new and emerging technologies that can improve the lives of its citizens. Being the largest city in the US, the challenges most cities face are compounded by scale. Many of these challenges revolve around city traffic and public transportation.

At the conclusion of their study, the city council realized that the Internet of Things (IoT) is widely available and is becoming more integrated into our daily lives. Fabrikam City can capitalize on the wide availability and affordability of IoT devices. This means physical things like traffic lights and vehicles will be able to collect and share data by connecting to the Internet. Through analytics, cities can turn this data into intelligent information that will change the way the world works.

They realize that IoT offers cities revolutionary ways to gather this vital data. This wave of change will enable innovation, new services, and cost savings. However, if Fabrikam City is going to be \"smart\" (that is, IoT-connected), it needs to devise a strategy for deploying technologies rather than allowing 1,000 ad-hoc systems to take root without forethought. A proactive strategy will maximize the return on an IoT investment.

Fabrikam City buses are widely used throughout the many boroughs, such as Northwind, Contoso, and Tailwind. It would make sense to somehow attach IoT devices to those buses to perform a number of functions such as preventive maintenance and tracking driver performance. It would be great if only the most important information is sent off over the internet instead of constantly sending a lot of data over expensive cellular connectivity. The most important information could be described as anomalies, like impending mechanical breakdowns or reckless driving. The buses will periodically send their location information that can be used to track their progress on their route. All of the information can be uploaded once the bus arrives at its home station where it has Wi-Fi connectivity. The full data could then be used for accounting and analysis of the data collected throughout the day.

In regard to traffic lights, Fabrikam city council would also like reduce physical inspections by adding IoT devices with sensors that can detect maintenance and performance issues, such as if the light sequence is stuck in a continuous cycle loop outside of normal parameters, voltage irregularities, etc. Unlike the city buses, these traffic lights are always connected to the internet and can send and receive command and control information as often as needed.

Relecloud has been contracted to build a cloud-based architecture based on these requirements. They have researched IoT-related service offerings from various cloud providers and are interested in recent capabilities added to Azure for storing and evaluating time series data, as well as building IoT devices that run on the edge for localized data transformation and processing.

"While bidding for the Fabrikam smart city contract, we made it a primary goal from the start to build a solution that can easily be taken over by the city's IT department," says Rodrigo Romani, CTO of Relecloud. "Localizing telemetry captured on buses, that can be evaluated on the fly, while at the same time controlling the flow of data over a slow or unreliable internet connection seems like a tall order. We need a solution that is repeatable for fleets of buses entering or leaving service and is manageable in a way that makes it easy to send out targeted updates to groups of buses at a time."

Following this same mandate of ease of use and future flexibility without overtaxing an already busy city IT department, Relecloud is seeking options for capturing and displaying time series data that allows the end user to easily combine all of the data in any number of ways, with advanced querying and filtering options, and that can automatically handle schema changes of incoming telemetry and new device types as they are added. Additionally, this data needs to be accessible by an external web application for managing alerts and device management and must be able to display the raw data within a given timeframe, without pre-aggregation, at scale.

### Customer needs

1.  Reduce bus maintenance costs by analyzing city bus telemetry and applying predictive analysis provided by a trained machine learning model.

2.  Detect other anomalies, such as bus driver behavior, that can be sent as needed.

3.  Reduce the amount of information transmitted to the cloud by buses, which use expensive cellular data.

4.  Store the bus telemetry data locally when offline and send when internet connectivity is available.

5.  Send regular location updates of city buses that can be used to display on a map and update bus routes status.

6.  Install IoT devices on traffic lights for maintenance purposes.

7.  Easily combine all of the time series data in a single pane of view, with advanced querying and filtering options, and that can automatically handle schema changes of incoming telemetry and new device types as they are added.

8.  Have a web portal to view the buses and traffic lights on a map, manage device provisioning, alert rules, reference data, and control messages.

### Customer objections

1.  If bus data is collected offline and sent later on, how can we be certain that the time series data is not displayed out of order from when it was captured?

2.  We would like to be able to have more than one service access the IoT data without leading to reuse by multiple readers, and other negative effects. Is this possible?

3.  We are concerned about rolling out updates to all edge devices at once, in case there's a problem. Can we deploy updates to small groups instead?

### Infographic for common scenarios

![A Common Scenario of Internet of Things flowchart is split between Azure and On-Premises. At a high level, Azure steps are: Ingest, Stream Processing, Batch Storage, Speed Serving, Batch Processing, Batch View Serving, and Analytics Clients.](images/Whiteboarddesignsessionstudentguide-IoTandtheSmartCityimages/media/image2.png 'Common Scenario for IoT')

## Step 2: Design a proof of concept solution

**Outcome**

Design a solution and prepare to present the solution to the target customer audience in a 15-minute chalk-talk format.

Timeframe: 60 minutes

**Business needs**

Directions: With all participants at your table, answer the following questions and list the answers on a flip chart:

1.  Who should you present this solution to? Who is your target customer audience? Who are the decision makers?

2.  What customer business needs do you need to address with your solution?

**Design**

Directions: With all participants at your table, respond to the following questions on a flip chart:

_High-level architecture_

1.  Without getting into the details (the following sections will address the particular details), diagram your initial vision for handling the top-level requirements for the IoT Smart City.

_IoT Devices_

1.  Which data ingest service would you use for this scenario, IoT Hub or Event Hubs? Be specific.

2.  How will you process the vehicle telemetry data locally to only send important data about driver performance or potential mechanical issues while the bus is in transit, with limited data connectivity?

3.  What would you recommend using to separately send bus location and speed data at regular intervals?

4.  Describe how you would send traffic light telemetry, and in turn, have the traffic lights receive commands to update their timing.

5.  Using your ingest service, how would you handle critical messages separately to route them to custom endpoints based on message properties?

_Data and visualization_

1.  Keeping the requirement for ease of use and flexibility in mind, how would you propose storing time series data for all the IoT devices? Is there a need for secondary storage?

2.  How will you allow end users to visualize the large amount of data in one place, which is flexible enough to handle changing data schemas and allow for ad-hoc queries and sharing views between users?

3.  Describe how the custom web application will be hosted, as well as how it will access the stored telemetry including displaying location information on a map, manage reference data (speed limit information, speed alert thresholds, vehicle information, etc.), alerts, and device control messages. Also, describe how you will use it to provision new IoT devices and send manual cloud-to-device messages.

4.  What would you recommend using to combine bus and traffic light information, filtering the data by the close proximity of both types of IoT devices, then by average bus speed information based on predefined speed thresholds?

_Location-based data and mapping_

1.  Relecloud wants to integrate map visualization to show the location of buses, traffic lights, and other devices. They would like to use a mapping service that does not require them to send data to an external resource, can be used with other Azure services like Stream Analytics, and supports data privacy and compliance when needed.

**Prepare**

Directions: With all participants at your table:

1.  Identify any customer needs that are not addressed with the proposed solution.

2.  Identify the benefits of your solution.

3.  Determine how you will respond to the customer's objections.

Prepare a 15-minute chalk-talk style presentation to the customer.

## Step 3: Present the solution

**Outcome**

Present a solution to the target customer audience in a 15-minute chalk-talk format.

Timeframe: 30 minutes

**Presentation**

Directions:

1.  Pair with another table.

2.  One table is the Microsoft team and the other table is the customer.

3.  The Microsoft team presents their proposed solution to the customer.

4.  The customer makes one of the objections from the list of objections.

5.  The Microsoft team responds to the objection.

6.  The customer team gives feedback to the Microsoft team.

7.  Tables switch roles and repeat Steps 2-6.

## Wrap-up

Timeframe: 15 minutes

Directions: Tables reconvene with the larger group to hear the facilitator/SME share the preferred solution for the case study.

## Additional references

|                                                                          |                                                                                                 |
| ------------------------------------------------------------------------ | :---------------------------------------------------------------------------------------------: |
| Hi-resolution version of blueprint                                       |                     <https://msdn.microsoft.com/dn630664#fbid=rVymR_3WSRo>                      |
| What is Azure IoT Edge?                                                  |              <https://docs.microsoft.com/en-us/azure/iot-edge/how-iot-edge-works>               |
| Understand the requirements and tools for developing IoT Edge modules    |              <https://docs.microsoft.com/en-us/azure/iot-edge/module-development>               |
| Deploy Azure Machine Learning as an IoT Edge module                      |       <https://docs.microsoft.com/en-us/azure/iot-edge/tutorial-deploy-machine-learning>        |
| Deploy and monitor IoT Edge modules at scale                             |             <https://docs.microsoft.com/en-us/azure/iot-edge/how-to-deploy-monitor>             |
| Understand and use device twins in IoT Hub                               |         <https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-device-twins>          |
| Routing messages with IoT Hub                                            |       <https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-process-d2c>        |
| Use message routes and custom endpoints for device-to-cloud messages     |     <https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-read-custom>      |
| Send cloud-to-device messages from IoT Hub                               |         <https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-messages-c2d>          |
| Schedule jobs on multiple devices                                        |             <https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-jobs>              |
| What is Azure Time Series Insights?                                      |   <https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-overview>   |
| Azure Time Series Insights explorer                                      |   <https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-explorer>   |
| About Azure Cosmos DB                                                    |                 <https://docs.microsoft.com/en-us/azure/cosmos-db/introduction>                 |
| Target Azure Cosmos DB for JSON output from Stream Analytics             |  <https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-documentdb-output>   |
| Using reference data or lookup tables in a Stream Analytics input stream |  <https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-use-reference-data>  |
| Run Azure Functions with Azure Stream Analytics jobs                     | <https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-with-azure-functions> |
| Azure Stream Analytics on IoT Edge                                       |         <https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-edge>         |
| GeoSpatial Functions                                                     |                    <https://msdn.microsoft.com/en-us/library/mt778980.aspx>                     |
| About Azure Location Based Services                                      | <https://docs.microsoft.com/en-us/azure/location-based-services/about-location-based-services>  |
| Using the Azure Location Based Services Search service                   |   <https://docs.microsoft.com/en-us/azure/location-based-services/how-to-search-for-address>    |
