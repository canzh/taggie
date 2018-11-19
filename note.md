# Taggie Design Thoughts

## Redis Models

### taggie:project:queue:size

type: string
int value to configure the queue size

### taggie:project:queue:123

type: list
stores at most 1000 project item ids with status=New

### taggie:user:queue:123456

type: string
stores current working item id for each user, the values are from taggie:project:queue:1

generation: peek the first element from project item list, retrieve metadata from database,
then stores them as json

expire: expired after a period of time to avoid

### taggie:online:user:count

type: string
stores current online user count

### taggie:project:statistics:123

type: hash
stores project statistics

total-items: 100
remaining-items: 50

### taggie:topused:keywords:123

type: sorted-set
stores top 5 frequently used keywords

### taggie:topused:categories:123

type: sorted-set
stores top 5 frequently used categories

### taggie:topused:subcategories:123

type: sorted-set
stores top 5 frequently used subcategories

### taggie:categories:123

type: list
fixed set of elements

### taggie:subcategories:123

type: list
fixed set of elements

### taggie:keywords:123

type: list
dynamicly changes when new keyword was added

## Backgroud Job

### project queue populate job

periodically check the project queue size and ensure it's keeping the fix size.
if less than that, query database for the project item ids and fill the queue.

### message queue consumer

handler for finished queue items

## Message Queue

after finish tagging a item, there are several thing to do:
1.update project item status
1.create finished-by record
1.add category link
1.add subcategory link
1.store keywords and add link
1.update taggie statistics
1.update team statistics
1.update taggie:topused:[type]