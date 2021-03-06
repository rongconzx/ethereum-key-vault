﻿open EthereumKeyVault

[<EntryPoint>]
let main argv =

    // Create a key for alice
    let aliceKey = 
        defaultKeyParams
        |> createKey "alice"
    
    printfn "Alice's address: %s" (getAddress aliceKey)

    // Create a key for bob
    let bobKey =
        defaultKeyParams
        |> createKey "bob"

    printfn "Bob's address: %s" (getAddress bobKey)

    // Define amount to send and nonce
    // Nonce must match the total transaction count for that address
    // Ex: In geth console check 'eth.getTransactionCount("alice's address")'
    // Or using the Rpc wrapper:
    // getTransactionCount (aliceKey |> getAddress) + any pending transactions by alice
    let nonce = bigint 0
    let amount = etherToWei (bigint 1)

    // Get the unsigned message array
    let message = 
        createTransaction (getAddress bobKey) amount nonce
        |> transactionToMessage

    // Construct the full offline transaction hash
    // In geth, use 'eth.sendRawTransaction("txHash")' to commit the transaction
    // Note that Alice has to have funds on her address.
    let txHash = 
        message
        |> signMessage aliceKey
        |> encodeRlp message
        |> toHex
        |> (+) "0x"

    // If running local geth, you can send the transaction directly.
    // Make sure RPC is running by setting --rpc flag
    // Ex: geth --datadir ~/.eth_test/ --maxpeers 0 --nodiscover --rpc console
    sendRawTransaction txHash
    |> printfn "Transaction %s sent"

    0 // return an integer exit code
