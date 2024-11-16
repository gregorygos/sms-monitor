# Hosting and Scaling Plan SMS-Monitor

---

Text messages are an incredible way to engage customers. Not only are phone numbers excellent ways to provide: additional account security, primary keys for user accounts (to prevent voucher abuse), but they also create an awesome opportunity for brands to engage with their clients.

Usecases include: Exclusive Offers, Loyalty Rewards and Birthday Specials. Feedback and surveys is also a great usecase.

## Deployment and Hosting

### Developer Experience
- Containerize the microservice using **Docker** to ensure consistency across development and production environments. This ensures reproducibility and simplifies deployments.

### Hosting Platform
- Deploy the service on **Azure Kubernetes Service (AKS)**, which provides:
  - **Auto-scaling** within defined thresholds (below) to handle traffic spikes.
  - Seamless **CI/CD integration** via GitHub Actions for efficient testing and deployment.
  - High availability and fault tolerance to ensure stability.

---

## Scaling Strategies 

### Divide and Conquer
- To scale horizontally:
  - Partition data into multiple `ConcurrentDictionaries` using relevant keys, such as:
    - **Country code**.
    - **Modulo of the phone number**, to evenly distribute load.
  - For advanced caching and expiration strategies, offload to a third-party service such as **Redis**, leveraging techniques like **Least Recently Used (LRU)** to optimize resource usage and reduce bottlenecks.

### Message Batching
- A common use-case is texting customers about a lunch offer just before lunch:
- If clients are queuing text messages, batch them to minimize the risk of exceeding account limits and avoid deadlocks. 
- Scale dynamically based on:
  - **Deadlock duration**: Monitoring frequency and length of deadlocks to trigger scaling actions.
  - **Resource utilization**: Trigger scaling during high CPU or key-usage (Described in Splunk).
  - **Predictable high-traffic periods**: Preemptively add resources, e.g., during lunchtime surges.

---

## Monitoring and Logging

### Tools and Logging Approach
- Use **Splunk** for centralized logging and monitoring of all key actions:
  - Key additions, updates, deletions.
  - Errors and exceptions.
  - Deadlock timings for performance insights.

### Analysis and Alerts
- Configure alerts to notify when:
  - Deadlocks exceed acceptable limits.
  - Specific clients have high service utilisation. Could indicate service over-use or abuse.
  - Thresholds for key counts or resource usage are reached, prompting scaling actions or blacklisting if necessary.

### Advanced Monitoring
- Use Datadog for application performance monitoring to:
- Track stack traces for bottleneck identification.
- Optimize slow operations. For instance, is our delete key appropriate and should we use LRU (Least Recently Used) instead?


## Run Tests
> `dotnet test SmsRelaySolution.sln`

## Docker
> `docker build -t sms-relay .`
> `docker run -d -p 4000:4000 sms-relay`