''' <summary>
''' Encoding:
''' EncodeTokenStr: Encodes a String token And returns its positional embedding As a list Of doubles.
'''    EncodeTokenEmbedding: Encodes a token embedding (list Of doubles) And returns its positional embedding As a list Of doubles.
'''    EncodeSentenceStr: Encodes a list Of String tokens And returns their positional embeddings As a list Of lists Of doubles.
'''    EncodeSentenceEmbedding: Encodes a list Of token embeddings And returns their positional embeddings As a list Of lists Of doubles.
'''Decoding:
'''DecodeTokenStr: Decodes a positional embedding (list Of doubles) And returns the corresponding String token.
'''    DecodeTokenEmbedding: Decodes a positional embedding (list Of doubles) And returns the corresponding token embedding As a list Of doubles.
'''    DecodeSentenceStr: Decodes a list Of positional embeddings And returns the corresponding String tokens As a list Of strings.
'''    DecodeSentenceEmbedding: Decodes a list Of positional embeddings And returns the corresponding token embeddings As a list Of lists Of doubles.
'''     </summary>
Public Class PositionalEncoderDecoder
    Private encodingMatrix As List(Of List(Of Double))
    Private Vocabulary As New List(Of String)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Dmodel">Embedding Model Size 
    ''' (1. often best to use the Vocabulary D_model)
    ''' (2. often a Fixed 512 is used LLM)
    ''' (3: 64 SMall LLM) </param>
    ''' <param name="MaxSeqLength">Max Sentence Length</param>
    ''' <param name="vocabulary">Known VocabularyList</param>
    Public Sub New(ByRef Dmodel As Integer, MaxSeqLength As Integer, vocabulary As List(Of String))
        '1. Create Embedding Matrix  Dmodel * MaxSeqLength
        CreateEmbeddingMatrix(Dmodel, MaxSeqLength)
        '2. Set Reference Vocabulary
        Me.Vocabulary = vocabulary
    End Sub

    'Encode
    Public Function EncodeTokenStr(ByRef nToken As String) As List(Of Double)
        Dim positionID As Integer = GetTokenIndex(nToken)
        Return If(positionID <> -1, encodingMatrix(positionID), New List(Of Double)())
    End Function

    Public Function EncodeTokenEmbedding(ByRef TokenEmbedding As List(Of Double)) As List(Of Double)
        Dim positionID As Integer = GetTokenIndex(TokenEmbedding)
        Return If(positionID <> -1, encodingMatrix(positionID), New List(Of Double)())
    End Function
    Public Function EncodeSentenceStr(ByRef Sentence As List(Of String)) As List(Of List(Of Double))
        Dim EncodedSentence As New List(Of List(Of Double))
        For Each Word In Sentence

            EncodedSentence.Add(EncodeTokenStr(Word))
        Next
        Return EncodedSentence
    End Function
    Public Function EncodeSentenceEmbedding(ByRef SentenceEmbeddings As List(Of List(Of Double))) As List(Of List(Of Double))
        Dim EncodedSentence As New List(Of List(Of Double))
        For Each Word In SentenceEmbeddings

            EncodedSentence.Add(EncodeTokenEmbedding(Word))
        Next
        Return EncodedSentence
    End Function

    'Decode
    Public Function DecodeSentenceStr(ByRef Sentence As List(Of List(Of Double))) As List(Of String)
        Dim DecodedSentence As New List(Of String)
        For Each Word In Sentence

            DecodedSentence.Add(DecodeTokenStr(Word))
        Next
        Return DecodedSentence
    End Function
    Public Function DecodeSentenceEmbedding(ByRef Sentence As List(Of List(Of Double))) As List(Of List(Of Double))
        Dim DecodedSentence As New List(Of List(Of Double))
        For Each Word In Sentence

            DecodedSentence.Add(DecodeTokenEmbedding(Word))
        Next
        Return DecodedSentence
    End Function
    ''' <summary>
    ''' Used For String Tokens
    ''' </summary>
    ''' <param name="PositionalEmbeddingVector"></param>
    ''' <returns>String Token</returns>
    Public Function DecodeTokenStr(ByRef PositionalEmbeddingVector As List(Of Double)) As String
        Dim positionID As Integer = GetPositionID(PositionalEmbeddingVector)
        Return If(positionID <> -1, Vocabulary(positionID), "")
    End Function
    ''' <summary>
    ''' USed to decode WOrdEMbedding Vectors instead of strings
    ''' </summary>
    ''' <param name="PositionalEmbeddingVector"></param>
    ''' <returns>WOrdEMbedding Vector</returns>
    Public Function DecodeTokenEmbedding(ByRef PositionalEmbeddingVector As List(Of Double)) As List(Of Double)
        Dim positionID As Integer = GetPositionID(PositionalEmbeddingVector)
        Return If(positionID <> -1, encodingMatrix(positionID), New List(Of Double)())
    End Function



    Private Sub CreateEmbeddingMatrix(ByRef WidthLength As Integer, HeightLength As Integer)
        encodingMatrix = New List(Of List(Of Double))
        ' Create the encoding matrix
        For pos As Integer = 0 To HeightLength - 1
            Dim encodingRow As List(Of Double) = New List(Of Double)()

            For i As Integer = 0 To WidthLength - 1
                Dim angle As Double = pos / Math.Pow(10000, (2 * i) / WidthLength)
                encodingRow.Add(Math.Sin(angle))
                encodingRow.Add(Math.Cos(angle))
            Next

            encodingMatrix.Add(encodingRow)
        Next
    End Sub
    'GetPos
    Private Function GetPositionID(PositionalEmbeddingVector As List(Of Double)) As Integer
        For i As Integer = 0 To encodingMatrix.Count - 1
            If PositionalEmbeddingVector.SequenceEqual(encodingMatrix(i)) Then
                Return i
            End If
        Next

        Return -1 ' Position ID not found
    End Function
    Private Function GetTokenIndex(PositionalEncoding As List(Of Double)) As Integer

        For i As Integer = 0 To encodingMatrix.Count - 1
            If PositionalEncoding.SequenceEqual(encodingMatrix(i)) Then
                Return i
            End If
        Next

        Return -1 ' Token not found
    End Function
    Private Function GetTokenIndex(token As String) As Integer

        Return Vocabulary.IndexOf(token)
    End Function
End Class
