using DocumentBuildCO.Request;
using FeCoEventos.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace FeCoEventos.Clients.SendEmail
{
    public class SendEmailRequest
    {
        [JsonPropertyName("template")]
        public Template Template { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("async")]
        public bool Async { get; set; }

        [JsonPropertyName("independent_delivery")]
        public bool Independent_Delivery { get; set; }

        [JsonPropertyName("compress_attachments")]
        public bool Compress_Attachments { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);

        public static SendEmailRequest FromJson(string data) => JsonConvert.DeserializeObject<SendEmailRequest>(data);


        public SendEmailRequest(string name_template, string name_supplier, string supplier_id, string name_customer,
                               string document_id, string event_name, string event_id, string event_type, string ls_link, string email,
                               string fromEmail, string business_line, AttachedDocumentResponse attachedDocument,
                               bool compressAttachments, Ambiente enviroment)
        {
            Template = new Template();
            Template.Name = name_template;
            Param actualParam = new Param { Name = "nombre_emisor", Content = name_supplier };
            Template.Params = new List<Param>();
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "nit_emisor", Content = supplier_id };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "nombre_receptor", Content = name_customer };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "numero_documento", Content = document_id };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "numero_evento", Content = event_id };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "tipo_evento", Content = event_type };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "url_enlace", Content = ls_link };
            Template.Params.Add(actualParam);
            actualParam = new Param { Name = "nombre_estado", Content = event_name };
            Template.Params.Add(actualParam);
            Message = new Message();

            //Lista de Destinatarios
            Message.To = new List<To>();

            string[] emailList = new string[0];

            if (email.Contains(";"))
            {
                //Verifico si el sepador en punto y coma
                emailList = email.Split(';');
            }
            else if (email.Contains(","))
            {
                //Verifico si el sepador en coma
                emailList = email.Split(',');
            }
            else
            {
                emailList = new string[1];
                emailList[0] = email;
            }

            if (emailList != null)
            {
                foreach (string row in emailList)
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        To actualTo = new To();
                        actualTo.Email = row.Trim();
                        actualTo.Name = name_supplier;
                        actualTo.Type = ToType.TO;

                        Message.To.Add(actualTo);
                    }
                }
            }

            Message.Subject = "Evento;" +
                (!String.IsNullOrEmpty(document_id) ? document_id + ";" : "") +
                (!String.IsNullOrEmpty(supplier_id) ? supplier_id + ";" : "") +
                (!String.IsNullOrEmpty(name_supplier) ? name_supplier + ";" : "") +
                (!String.IsNullOrEmpty(event_id) ? event_id + ";" : "") +
                (!String.IsNullOrEmpty(event_type) ? event_type + ";" : "") +
                (!String.IsNullOrEmpty(business_line) ? business_line : "");
            Message.From_Email = new FromEmail
            {
                Email = fromEmail,
                Name = "The Factory HKA Colombia",
            };

            Message.Headers = new Headers
            {
                ReplyTo = fromEmail
            };
            Message.Attachments = new List<Attachment>();
            if (attachedDocument != null)
            {
                Attachment attachment = new Attachment();
                attachment.Filename = attachedDocument.Namefile;
                attachment.Content = attachedDocument.File;
                Message.Attachments.Add(attachment);
            }
            Async = true;
            Independent_Delivery = false;
            Compress_Attachments = compressAttachments;
        }
    }

    public class Template
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("params")]
        public List<Param> Params { get; set; }
    }

    public class Param
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("from_email")]
        public FromEmail From_Email { get; set; }

        [JsonPropertyName("to")]
        public List<To> To { get; set; }

        [JsonPropertyName("headers")]
        public Headers Headers { get; set; }

        [JsonPropertyName("attachments")]
        public List<Attachment> Attachments { get; set; }
    }
    public class Attachment
    {
        public Attachment()
        {
            FilePath = "";
            ZipPath = "/";
        }

        [JsonPropertyName("status")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public AttachmentStatus Status { get; set; }

        [JsonPropertyName("filename")]
        public string Filename { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("file_path")]
        public string FilePath { get; set; }

        [JsonPropertyName("zip_path")]
        public string ZipPath { get; set; }
    }

    [DataContract]
    public class Headers
    {
        [JsonPropertyName("Reply-To")]
        public string ReplyTo { get; set; }
    }
    [DataContract]
    public enum AttachmentStatus
    {
        [EnumMember(Value = "GUARDADO")]
        GUARDADO,
        [EnumMember(Value = "ELIMINADO")]
        ELIMINADO,
        [EnumMember(Value = "ERROR")]
        ERROR
    }
    public class To
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public ToType Type { get; set; }
    }

    [DataContract]
    public enum ToType
    {
        [EnumMember(Value = "TO")]
        TO,
        [EnumMember(Value = "CC")]
        CC,
        [EnumMember(Value = "BCC")]
        BCC
    }

    [DataContract]
    public class FromEmail
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
}
