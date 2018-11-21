# Taggie Design Thoughts

## Redis Models

### taggie:project:queue:size

type: string
int value to configure the queue size

### taggie:project:queue:{projectid}

type: list
stores at most 1000 project item ids with status=New

### taggie:user:queue:{projectid}:{userid}

type: sorted-set
stores current work in progress item id for each user, with timestampe as score

generation: peek the first element from project item list, retrieve metadata from database,
then stores them as json

expire: expired after a period of time to avoid

### taggie:online:user:count

type: string
stores current online user count

### taggie:project:metadata:{projectid}

type: hash
stores project statistics

project-name: project1
total-items: 100
remaining-items: 50

### taggie:topused:keywords:{projectid}

type: sorted-set
stores top 5 frequently used keywords

### taggie:topused:categories:{projectid}

type: sorted-set
stores top 5 frequently used categories

### taggie:topused:subcategories:{projectid}

type: sorted-set
stores top 5 frequently used subcategories

### taggie:categories:{projectid}

type: list
fixed set of elements

### taggie:subcategories:{projectid}

type: list
fixed set of elements

### taggie:keywords:{projectid}

type: sorted-set
dynamicly changes when new keyword was added

### taggie:team:statistics:{projectid}:{teamid}

type: hash
record team users effort statistics as well as whole team

sample:
{
    "team:type": taggie,
    "team:finished": 1000,
    "team:incorrect": 100,
    "1:finished": 200,
    "1:incorrect": 5,
    "2:finished": 500,
    "2:incorrect": 10,
    ...
}

## Backgroud Job

### project queue populate job

periodically check the project queue size and ensure it's keeping the fix size.
if less than that, query database for the project item ids and fill the queue.

### message queue consumer

handler for finished queue items

## Message Queue

after finish tagging a item, there are several thing to do:

1. update project item status
1. create finished-by record: projectitemeffort
1. add category link
1. add subcategory link
1. store keywords
1. update taggie statistics
1. update team statistics
1. delete project item id in project wip queue
1. delete project item id in user queue
