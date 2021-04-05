# Important - Please Note
The IoT and the Smart City lab documents have been removed due to technical issues.  The IoT team is currently working on an Azure Digital Twins workshop, to be published May 2021, as a replacement.

# IoT and the Smart City

Fabrikam City council has conducted a six-month study of new and emerging technologies that can improve the lives of its citizens. Being the largest city in the US, the challenges most cities face are compounded by scale. Many of these challenges revolve around city traffic and public transportation.

At the conclusion of their study, the city council realized that the Internet of Things (IoT) is widely available and is becoming more integrated into our daily lives. Fabrikam City can capitalize on the wide availability and affordability of IoT devices. This means physical things like traffic lights and vehicles will be able to collect and share data by connecting to the Internet. Through analytics, cities can turn this data into intelligent information that will change the way the world works.

June 2020

## Target audience
- Application developer
- Data engineer

## Abstracts

### Workshop

In this workshop you will use the unique benefits of the Internet of Things (IoT) to build a smart city solution to help improve traffic and public transportation in Fabrikam City. Use a combination of the power of the cloud, along with IoT Edge devices to provide anomaly detection from city buses, including engine anomaly alerts and aggressive driving detection, as well as location broadcasting to update bus route status. Traffic lights will also receive new IoT devices that can help detect maintenance and performance issues, such as voltage irregularities. Easily view all of this information through a centralized reporting dashboard provided by the Azure Remote Monitoring Accelerator web application. Use the IoT Remote Monitoring accelerator to manage and simulate IoT devices, set alerts, and view data on a map.

By the end of this workshop, you will learn to use IoT Hub to manage IoT devices, configure and run the IoT Remote Monitoring accelerator to provision, manage, and simulate telemetry for IoT devices, use Azure IoT Edge to collect vehicle telemetry data, detect anomalies, and send the summarized data to Azure IoT Hub as needed.  In addition, you'll route critical alerts to a Service Bus Queue, create an Azure function that extracts critical alerts from the Service Bus Queue and stores them in Cosmos DB, as well as use Azure Time Series Insights to store, visualize, and query the large amounts of time series data generated by various IoT devices.

### Whiteboard design session

This whiteboard design session is designed to help you gain a better understanding of implementing architectures that use IoT data in new and innovative ways. You will design an IoT workflow that begins with a local IoT edge device that collects and analyzes data from various sensors that are connected to it, and intelligently aggregates and sends that data to the cloud when anomalies are detected, other non-critical data is uploaded to cloud storage when a viable internet connection is available. Once the data is uploaded, it is sent to a time-series database for rapid analysis alongside other classes of IoT data to spot and act on correlated information in real-time. You will also configure alerts when certain thresholds are exceeded and configure a remote monitoring solution that manages and sends control messages to IoT devices located within the city limits.

At the end of this whiteboard design session, you will be better able to design an end-to-end IoT solution that processes and analyzes data both in the field and in the cloud.

### Hands-on lab

We have removed the IoT and the Smart City lab documents due to technical issues.  Please look for an update in the upcoming months.

## Azure services and related products
- Azure IoT Hub
- Azure IoT Edge
- Azure Stream Analytics
- Azure Time Series Insights
- Azure Storage
- Azure Event Hub
- Azure Service Bus
- Visual Studio
- Visual Studio Code

## Azure solutions
Internet of Things

## Related references
- [MCW](https://microsoftcloudworkshop.com)

## Help & Support

We welcome feedback and comments from Microsoft SMEs & learning partners who deliver MCWs.  

***Having trouble?***
- First, verify you have followed all written lab instructions (including the Before the Hands-on lab document).
- Next, submit an issue with a detailed description of the problem.
- Do not submit pull requests. Our content authors will make all changes and submit pull requests for approval.

If you are planning to present a workshop, *review and test the materials early*! We recommend at least two weeks prior.

### Please allow 5 - 10 business days for review and resolution of issues.

