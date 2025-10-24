



- [introduzione](#introduzione)
- [stack tecnologico](#stack%20tecnologico)
- [classi](#classi)



# INTRODUZIONE

Quiz Engine è un generatore di quiz che consente di creare e svolgere quiz direttamente da interfaccia web.  
Il sistema è progettato per ospitare banche dati di domande, chiamate _quiz_, dalle quali è possibile generare pool casuali configurabili, detti _quizseed_.  
Ogni _quizseed_, una volta creato, viene memorizzato e può essere completato più volte. Al termine di ogni sessione viene assegnato un punteggio che confluisce nella classifica personale di quel _quizseed_; il miglior punteggio di ciascun giocatore viene inoltre inserito nella classifica globale.

I punteggi sono calcolati in base al valore delle domande (ad esempio: facile=1 punto, difficile=2) e vengono definiti durante la fase di creazione o modifica del quiz.  
Durante la configurazione della sessione (_quizseed_), si può stabilire il numero totale di domande e quante includerne per ciascun livello di difficoltà. In questa fase si sceglie anche la modalità di gioco: **Sfida** o **Verifica**.  
Nella modalità _Sfida_ viene presentata una domanda alla volta, mentre nella modalità _Verifica_ tutte le domande vengono mostrate contemporaneamente.

Sono inoltre previste opzioni aggiuntive, come la gestione del tempo di completamento e alcune funzionalità specifiche per la modalità _Sfida_.  
Durante la creazione o modifica del quiz è possibile assegnare un tempo di risposta per singola domanda e un tempo totale per la sessione (impostabile dal _quizseed_). Si potrà decidere se far prevalere il tempo totale sulla somma dei tempi delle singole domande, oppure viceversa.  
In modalità _Sfida_ si potrà scegliere se utilizzare il tempo residuo guadagnato rispondendo velocemente per le domande successive, e se sarà possibile tornare indietro dopo aver risposto.

Le domande potranno avere più risposte corrette: in caso di risposta parziale verrà assegnato un punteggio proporzionalmente ridotto.  
Sarà possibile definire **sequenze di domande**, ossia gruppi di quesiti che devono essere presentati insieme e in un ordine preciso. Se una sequenza viene inclusa in un _pool_, tutte le domande che la compongono verranno automaticamente estratte e mostrate nella sequenza stabilita.

Al contrario, le **varianti** rappresentano insiemi di domande molto simili tra loro: in questo caso, se una di esse viene selezionata, le altre appartenenti allo stesso gruppo verranno escluse, garantendo che in ogni sessione compaia solo una variante.

Le sessioni generate dai _quizseed_ rispetteranno le configurazioni definite, ma — se la banca dati è sufficientemente ampia — saranno comunque diverse tra loro grazie alla generazione casuale.  
Se però si desidera conservare una copia esatta di uno specifico _pool_, sarà possibile salvarlo: in tal caso diventerà ripetibile e condivisibile.


# STACK TECNOLOGICO

per questo progetto ho deciso di utilizzare 
- .NET per il backend 
- SQL Server per il database
- React per il frontend
- postman e playwright per i test

# CLASSI


## QUIZ

la banca dati con tutte le domande e le risposte, con il valore associato a ciascun livello di difficoltà.
- nome
- domande
- valori difficoltà
## DOMANDA

la domanda e le sue risposte, giuste e sbagliate. opzionalmente potrò avere un tempo di risposta e dei segni di appartenenza a una sequenza o ad una variante

- domanda
- difficoltà (opzionale)
- risposte giuste
- risposte sbagliate
- sequenza e numero sequenza (opzionale)
- variante (opzionale)
- tempo risposta (opzionale)
## QUIZ SEED

configurazione della sessione, contiene informazioni come il suo nome, da quale quiz prendere le domande, quante prenderne, che modalità. opzionalmente si può specificare quante domande per ogni difficoltà, se fare una sessione a tempo, se scegliere un tempo totale o la somma del tempo delle singole domande, e in modalità sfida: se poter tornare indietro dopo aver risposto e se poter usare lo scarto di tempo guadagnato per rispondere alle domande successive

- che quiz fare
- quante domande
- modalità (sfida o verifica)
- a tempo (opzionale, di default non a tempo)
- quante per ogni difficoltà (opzionale)
- tempo totale impostato o somma tempo singole domande (opzionale)

solo in modalità sfida
 - possibilità di tornare indietro
 - possibilità di sfruttare lo scarto di tempo ottenuto

## SCOREBOARD

2 tabelle per quizseed, una personale ed una globale, segnano i risultati delle varie sessioni 
e la media della tabella

- punteggio
- data sessione
- nome giocatore (opzionale, solo nella globale viene visto)
- media
## SESSIONE

la sessione di test che si sta svolgendo, tiene traccia del tempo, delle risposte date e del nome di chi sta svolgendo il test

- nome giocatore
- risposte date
- tempo impiegato e tempo rimanente

## GIOCATORE

colui che svolge i test e ottiene i punteggi

- nome 

## QUIZ CONTROLLER

il controller che espone le chiamate al frontend che copriranno le seguenti funzionalità

- login 
- logout
- registrazione nuovo utente
- crea quiz
- modifica quiz
- mostra quiz
- mostra mome di tutti i quiz
- mostra quizseed
- mostra nome di tutti i quizseed
- crea quizseed
- mostra scoreboard globale
- mostra scoreboard personale
- inizia sessione 
- invia risultato
- elimina quiz
- elimina quizseed

## QUIZ SERVICE

il service a cui si appoggia il controller contiene la logica di buisness dell'applicativo

## DB SERVICE

il service che esegue le operazioni sul database

## DB BASE SERVICE
 
il servizio che viene esteso da dbservice, contiene metodi per eseguire query e operazioni in sicurezza e con le transactions

